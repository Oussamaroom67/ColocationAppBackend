// BL/DashboardStudentService.cs
using ColocationAppBackend.Data;
using ColocationAppBackend.DTOs.Responses;
using ColocationAppBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace ColocationAppBackend.BL
{
    public class DashboardStudentService
    {
        private readonly ApplicationDbContext _context;

        public DashboardStudentService(ApplicationDbContext context)
        {
            _context = context;
        }
        // return 
        public async Task<DashboardStudentDto> GetDashboardStatsAsync(int etudiantId)
        {
            // Vérifier que l'étudiant existe
            var etudiant = await _context.Etudiants
                .FirstOrDefaultAsync(e => e.Id == etudiantId);

            if (etudiant == null)
                throw new ArgumentException("Étudiant non trouvé");

            // Compter les propriétés enregistrées (colocations créées par l'étudiant)
            var mesColocations = await _context.Colocations
                .CountAsync(c => c.EtudiantId == etudiantId);

            // Compter les messages non lus dans toutes les conversations de l'étudiant
            var messagesNonLus = await _context.Messages
                .Where(m => m.Conversation.Utilisateur1Id == etudiantId || m.Conversation.Utilisateur2Id == etudiantId)
                .Where(m => m.ExpediteurId != etudiantId) // Messages reçus seulement
                .Where(m => !m.EstLu)
                .CountAsync();

            // Compter les demandes de colocataires (demandes reçues sur les colocations de l'étudiant)
            var demandesDeColocataires = await _context.DemandesColocation
                .Where(dc => dc.EtudiantId == etudiantId && dc.Statut==0)
                .CountAsync();

            // Compter les favoris de l'étudiant
            var proprietesEnregistrees = await _context.Favoris
                .CountAsync(f => f.EtudiantId == etudiantId);

            return new DashboardStudentDto
            {
                ProprietesEnregistrees = proprietesEnregistrees,
                MessagesNonLus = messagesNonLus,
                DemandesDeColocataires = demandesDeColocataires,
                MesColocations = mesColocations
            };
        }

        public async Task<List<MessageRecentDto>> GetRecentMessagesAsync(int etudiantId)
        {
            // Vérifier que l'étudiant existe
            var etudiant = await _context.Etudiants
                .FirstOrDefaultAsync(e => e.Id == etudiantId);
            if (etudiant == null)
                throw new ArgumentException("Étudiant non trouvé");

            // Récupérer les 3 messages récents reçus
            var messagesRecents = await _context.Messages
                .Where(m => m.Conversation.Utilisateur1Id == etudiantId || m.Conversation.Utilisateur2Id == etudiantId)
                .Where(m => m.ExpediteurId != etudiantId) // Messages reçus seulement
                .Include(m => m.Expediteur)
                .OrderByDescending(m => m.DateEnvoi)
                .Take(3)
                .Select(m => new MessageRecentDto
                {
                    Id = m.Id,
                    Name = m.Expediteur.Prenom + " " + m.Expediteur.Nom,
                    Message = m.Contenu,
                    Date = m.DateEnvoi.ToString("yyyy-MM-dd"),
                    Role = m.Expediteur is Etudiant ? "etudiant" : "proprietaire"
                })
                .ToListAsync();

            return messagesRecents;
        }

        public async Task<List<PropertyRecentDto>> GetRecentPropertiesAsync(int etudiantId)
        {
            // Vérifier que l'étudiant existe
            var etudiant = await _context.Etudiants
                .FirstOrDefaultAsync(e => e.Id == etudiantId);
            if (etudiant == null)
                throw new ArgumentException("Étudiant non trouvé");

            // Récupérer les 3 dernières annonces publiées récemment
            var propertiesRecentes = await _context.Annonces
                .Include(a => a.Logement)
                .Include(a => a.Photos)
                .OrderByDescending(a => a.DateModification)
                .Take(3)
                .Select(a => new PropertyRecentDto
                {
                    Id = a.Id,
                    Title = a.Titre,
                    Location = a.Logement.Ville + ", " + a.Logement.Pays,
                    Price = a.Prix.ToString("0"),
                    Type = a.Logement.Type,
                    Image = a.Photos.FirstOrDefault() != null ? a.Photos.FirstOrDefault().Url : "/src/assets/images/home.jpg"
                })
                .ToListAsync();

            return propertiesRecentes;
        }
    }
}