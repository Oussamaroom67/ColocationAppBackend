using ColocationAppBackend.Data;
using ColocationAppBackend.DTOs.Responses;
using ColocationAppBackend.Enums;
using Microsoft.EntityFrameworkCore;

namespace ColocationAppBackend.BL
{
    public class DashboardOwnerService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private string baseUrl;
        public DashboardOwnerService(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
            this.baseUrl = _configuration["BaseUrl"];
        }

        public async Task<DashboardStatsDto> GetDashboardStatsAsync(int proprietaireId)
        {
            // Récupérer les annonces actives du propriétaire
            var annoncesActives = await _context.Annonces
                .Include(a => a.Logement)
                .Where(a => a.Logement.ProprietaireId == proprietaireId &&
                           a.Statut == AnnonceStatus.Active)
                .CountAsync();

            // Récupérer les nouvelles demandes (en attente)
            var nouvellesDemandes = await _context.DemandesLocation
                .Include(d => d.Annonce)
                .Include(d => d.Annonce.Logement)
                .Where(d => d.Annonce.Logement.ProprietaireId == proprietaireId &&
                           d.status == LocationStatus.EnAttente)
                .CountAsync();

            // Calculer le nombre total de vues
            var totalVues = await _context.Annonces
                .Include(a => a.Logement)
                .Where(a => a.Logement.ProprietaireId == proprietaireId)
                .SumAsync(a => a.NbVues);

            // Compter les locataires potentiels (nombre unique d'étudiants ayant fait des demandes)
            var locatairesPotentiels = await _context.DemandesLocation
                .Include(d => d.Annonce)
                .Include(d => d.Annonce.Logement)
                .Where(d => d.Annonce.Logement.ProprietaireId == proprietaireId)
                .Select(d => d.EtudiantId)
                .Distinct()
                .CountAsync();

            return new DashboardStatsDto
            {
                AnnoncesActives = annoncesActives,
                NouvellesDemandes = nouvellesDemandes,
                TotalVues = totalVues,
                LocatairesPotentiels = locatairesPotentiels
            };
        }

        public async Task<List<ProprieteCardDto>> GetProprietesAsync(int proprietaireId)
        {
            var proprietes = await _context.Annonces
                .Include(a => a.Logement)
                .Include(a => a.Photos)
                .Where(a => a.Logement.ProprietaireId == proprietaireId)
                .Select(a => new ProprieteCardDto
                {
                    Id = a.Id,
                    Titre = a.Titre,
                    Prix = a.Prix,
                    Statut = a.Statut.ToString(),
                    NbVues = a.NbVues,
                    NbDemandes = a.DemandesLocation.Count(),
                    PhotoUrl = a.Photos.Any()
                    ? $"{baseUrl}{a.Photos.First().Url}"
                    : "https://wiratthungsong.com/wts/assets/img/default.png",
                    LogementId = a.LogementId,
                    DateModification = a.DateModification
                })
                .OrderByDescending(p => p.DateModification)
                .ToListAsync();

            return proprietes;
        }

        public async Task<List<RecentInquiryDto>> GetRecentInquiriesAsync(int proprietaireId, int limit = 3)
        {
            var recentInquiries = await _context.DemandesLocation
                .Include(d => d.Annonce)
                .Include(d => d.Annonce.Logement)
                .Include(d => d.Etudiant)
                .Where(d => d.Annonce.Logement.ProprietaireId == proprietaireId)
                .OrderByDescending(d => d.DateCreation)
                .Take(limit)
                .Select(d => new RecentInquiryDto
                {
                    Id = d.Id,
                    NomEtudiant = $"{d.Etudiant.Prenom} {d.Etudiant.Nom}",
                    InitialeEtudiant = d.Etudiant.Prenom.Substring(0, 1).ToUpper(),
                    TitrePropriete = d.Annonce.Titre,
                    Message = d.Message,
                    DateCreation = d.DateCreation,
                    EstRepondu = d.status != LocationStatus.EnAttente,
                    Statut = d.status == LocationStatus.EnAttente ? "Nouveau" : "Répondu",
                    AnnonceId = d.AnnonceId,
                    EtudiantId = d.EtudiantId
                })
                .ToListAsync();

            return recentInquiries;
        }

        public async Task<PropertyAnalyticsDto> GetPropertyAnalyticsAsync(int proprietaireId)
        {
            // Récupérer les performances des propriétés
            var propertyPerformances = await _context.Annonces
                .Include(a => a.Logement)
                .Include(a => a.DemandesLocation)
                .Where(a => a.Logement.ProprietaireId == proprietaireId)
                .Select(a => new PropertyPerformanceDto
                {
                    Id = a.Id,
                    Name = a.Titre,
                    Views = a.NbVues,
                    RequestsCount = a.DemandesLocation.Count()
                })
                .OrderByDescending(p => p.Views)
                .ToListAsync();

            // Calculer les statistiques de réponse
            var responseStats = await CalculateResponseStatsAsync(proprietaireId);

            return new PropertyAnalyticsDto
            {
                PropertyPerformances = propertyPerformances,
                ResponseRate = responseStats.ResponseRate,
                AverageResponseTimeHours = responseStats.AverageResponseTimeHours
            };
        }

        private async Task<ResponseStatsDto> CalculateResponseStatsAsync(int proprietaireId)
        {
            // Récupérer toutes les demandes de location du propriétaire
            var demandes = await _context.DemandesLocation
                .Include(d => d.Annonce)
                .Include(d => d.Annonce.Logement)
                .Where(d => d.Annonce.Logement.ProprietaireId == proprietaireId)
                .ToListAsync();

            if (!demandes.Any())
            {
                return new ResponseStatsDto
                {
                    ResponseRate = 0,
                    AverageResponseTimeHours = 0
                };
            }

            // Calculer le taux de réponse (demandes avec réponse / total demandes)
            var demandesAvecReponse = demandes.Where(d =>
                d.status == LocationStatus.Accepté ||
                d.status == LocationStatus.Refusée).ToList();

            var responseRate = demandes.Count() > 0
                ? (double)demandesAvecReponse.Count() / demandes.Count() * 100
                : 0;

            // Calculer le temps de réponse moyen
            var responseTimes = demandesAvecReponse
                .Where(d => d.DateReponse != default && d.DateCreation != default)
                .Select(d => (d.DateReponse - d.DateCreation).TotalHours)
                .ToList();

            var averageResponseTime = responseTimes.Any()
                ? responseTimes.Average()
                : 0;

            return new ResponseStatsDto
            {
                ResponseRate = Math.Round(responseRate, 1),
                AverageResponseTimeHours = Math.Round(averageResponseTime, 1)
            };
        }

        public async Task<List<RecentActivityDto>> GetRecentActivitiesAsync(int proprietaireId, int limit = 10)
        {
            var activities = new List<RecentActivityDto>();
            var now = DateTime.Now;

            // 1. Récupérer les nouvelles demandes (messages)
            var messagesData = await _context.DemandesLocation
                .Include(d => d.Annonce)
                .Include(d => d.Annonce.Logement)
                .Include(d => d.Etudiant)
                .Where(d => d.Annonce.Logement.ProprietaireId == proprietaireId)
                .OrderByDescending(d => d.DateCreation)
                .Take(limit)
                .Select(d => new {
                    d.Id,
                    d.DateCreation,
                    EtudiantPrenom = d.Etudiant.Prenom,
                    EtudiantNom = d.Etudiant.Nom,
                    AnnonceTitre = d.Annonce.Titre
                })
                .ToListAsync();

            foreach (var data in messagesData)
            {
                activities.Add(new RecentActivityDto
                {
                    Id = data.Id,
                    Type = "message",
                    Title = "Nouvelle demande reçue",
                    Description = $"{data.EtudiantPrenom} {data.EtudiantNom} a envoyé une demande de renseignements sur {data.AnnonceTitre}",
                    Time = FormatTimeAgo(data.DateCreation, now),
                    CreatedAt = data.DateCreation
                });
            }

            // 2. Récupérer les mises à jour d'annonces
            var updatesData = await _context.Annonces
                .Include(a => a.Logement)
                .Where(a => a.Logement.ProprietaireId == proprietaireId)
                .OrderByDescending(a => a.DateModification)
                .Take(limit)
                .Select(a => new {
                    a.Id,
                    a.DateModification,
                    a.Titre,
                    a.Statut
                })
                .ToListAsync();

            foreach (var data in updatesData)
            {
                activities.Add(new RecentActivityDto
                {
                    Id = data.Id,
                    Type = "update",
                    Title = "Statut de la propriété mis à jour",
                    Description = $"Vous avez mis à jour « {data.Titre} » - Statut: {GetStatutText(data.Statut)}",
                    Time = FormatTimeAgo(data.DateModification, now),
                    CreatedAt = data.DateModification
                });
            }

            // 3. Simuler les vues récentes
            var viewsData = await _context.Annonces
                .Include(a => a.Logement)
                .Where(a => a.Logement.ProprietaireId == proprietaireId && a.NbVues > 0)
                .OrderByDescending(a => a.NbVues)
                .Take(3)
                .Select(a => new {
                    a.Id,
                    a.DateModification,
                    a.Titre,
                    a.NbVues
                })
                .ToListAsync();

            foreach (var data in viewsData)
            {
                activities.Add(new RecentActivityDto
                {
                    Id = data.Id,
                    Type = "view",
                    Title = "Propriété visitée",
                    Description = $"Votre annonce « {data.Titre} » a reçu {data.NbVues} vues",
                    Time = FormatTimeAgo(data.DateModification, now),
                    CreatedAt = data.DateModification
                });
            }

            // Trier par date de création et limiter
            return activities
                .OrderByDescending(a => a.CreatedAt)
                .Take(limit)
                .ToList();
        }

        private static string FormatTimeAgo(DateTime dateTime, DateTime now)
        {
            var timeSpan = now - dateTime;

            if (timeSpan.Days > 7)
                return $"il y a {timeSpan.Days / 7} semaine{(timeSpan.Days / 7 > 1 ? "s" : "")}";
            if (timeSpan.Days > 0)
                return $"il y a {timeSpan.Days} jour{(timeSpan.Days > 1 ? "s" : "")}";
            if (timeSpan.Hours > 0)
                return $"il y a {timeSpan.Hours} heure{(timeSpan.Hours > 1 ? "s" : "")}";
            if (timeSpan.Minutes > 0)
                return $"il y a {timeSpan.Minutes} minute{(timeSpan.Minutes > 1 ? "s" : "")}";

            return "À l'instant";
        }

        private static string GetStatutText(AnnonceStatus statut)
        {
            return statut switch
            {
                AnnonceStatus.Active => "Actif",
                AnnonceStatus.Brouillon => "Inactif",
                AnnonceStatus.Louee => "Louée",
                _ => "Inconnu"
            };
        }
    }

}
