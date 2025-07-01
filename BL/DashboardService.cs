using ColocationAppBackend.Data;
using ColocationAppBackend.DTOs.Responses;
using ColocationAppBackend.Enums;
using Microsoft.EntityFrameworkCore;

namespace ColocationAppBackend.BL
{
    public class DashboardService
    {
        private readonly ApplicationDbContext _context;

        public DashboardService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<DashboardResponse> GetDashboardOverviewAsync()
        {
            var today = DateTime.Today;
            var yesterday = today.AddDays(-1);
            var lastMonth = today.AddMonths(-1);

            // Stats de base
            var totalUsers = await _context.Utilisateurs.CountAsync();
            var usersLastMonth = await _context.Utilisateurs.CountAsync(u => u.DateInscription >= lastMonth);
            var userChange = CalculatePercentageChange(usersLastMonth, totalUsers);

            var totalProperties = await _context.Logements.CountAsync();
            var propertiesLastMonth = await _context.Annonces.CountAsync(p => p.DateModification >= lastMonth);
            var propertyChange = CalculatePercentageChange(propertiesLastMonth, totalProperties);

            var messagesToday = await _context.Messages.CountAsync(m => m.DateEnvoi.Date == today);
            var messagesYesterday = await _context.Messages.CountAsync(m => m.DateEnvoi.Date == yesterday);
            var messageChange = CalculatePercentageChange(messagesYesterday, messagesToday);

            var pendingReports = await _context.Signalments.CountAsync(s => s.Statut == SignalementType.EnAttente);
            var newReportsToday = await _context.Signalments.CountAsync(s => s.DateSignalement.Date == today);

            var unverifiedUsers = await _context.Utilisateurs.CountAsync(u => !u.EstVerifie);

            // Répartition
            var etudiants = await _context.Etudiants.CountAsync();
            var proprietaires = await _context.Proprietaires.CountAsync();
            var admins = await _context.Administrateurs.CountAsync();

            // 5 dernières activités
            var lastLogements = await _context.Annonces
                .Include(l => l.Logement.Proprietaire)
                .OrderByDescending(l => l.DateModification)
                .Take(5)
                .Select(l => new ActivityDTO
                {
                    FullName = l.Logement.Proprietaire.Nom + " " + l.Logement.Proprietaire.Prenom,
                    Role = "Propriétaire",
                    Action = $"a ajouté une annonce : '{l.Titre}'",
                    Date = l.DateModification,
                })
                .ToListAsync();

            var lastDemandes = await _context.DemandesColocation
                .Include(d => d.Etudiant)
                .OrderByDescending(d => d.DateCreation)
                .Take(5)
                .Select(d => new ActivityDTO
                {
                    FullName = d.Etudiant.Nom + " " + d.Etudiant.Prenom,
                    Role = "Étudiant",
                    Action = "a créé une nouvelle demande de colocation",
                    Date = d.DateCreation
                })
                .ToListAsync();

            // Fusionner les deux listes et trier par date
            var allActivities = lastLogements
                .Concat(lastDemandes)
                .OrderByDescending(a => a.Date)
                .Take(5) 
                .ToList();


            // Signalements récents
            var recentReports = await _context.Signalments
                .OrderByDescending(s => s.DateSignalement)
                .Take(5)
                .Select(s => new RecentReportDTO
                {
                    Titre = s.Motif,
                    Statut = s.Statut.ToString(),
                    SignalePar = s.Signaleur.Nom + " " + s.Signaleur.Prenom,
                    Description = s.Description
                })
                .ToListAsync();

            return new DashboardResponse
            {
                TotalUsers = totalUsers,
                UserChange = userChange,
                TotalProperties = totalProperties,
                PropertyChange = propertyChange,
                MessagesToday = messagesToday,
                MessageChange = messageChange,
                PendingReports = pendingReports,
                NewReportsToday = newReportsToday,
                UnverifiedUsers = unverifiedUsers,
                UserDistribution = new UserDistributionDTO
                {
                    Etudiants = etudiants,
                    Proprietaires = proprietaires,
                    Admins = admins
                },
                LastActivities = allActivities,
                RecentReports = recentReports
            };
        }

        private string CalculatePercentageChange(int oldCount, int newCount)
        {
            if (oldCount == 0) return "+100%";
            var change = ((double)(newCount - oldCount) / oldCount) * 100;
            return change >= 0 ? $"+{Math.Round(change)}%" : $"{Math.Round(change)}%";
        }

    }
}
