// BL/DashboardStudentService.cs
using ColocationAppBackend.Data;
using ColocationAppBackend.DTOs.Responses;
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
                .Where(dc => dc.Colocation.EtudiantId == etudiantId)
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
    }
}