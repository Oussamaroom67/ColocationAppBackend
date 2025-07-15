using ColocationAppBackend.Data;
using ColocationAppBackend.DTOs.Responses;
using ColocationAppBackend.Enums;
using Microsoft.EntityFrameworkCore;

namespace ColocationAppBackend.BL
{
    public class OwnerStatsService
    {
        private readonly ApplicationDbContext _context;

        public OwnerStatsService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<OwnerStatsResponseDTO> GetStatsAsync(int proprietaireId)
        {
            // 1. Récupérer toutes les conversations où le propriétaire participe
            var conversationIds = await _context.Conversations
                .Where(c => c.Utilisateur1Id == proprietaireId || c.Utilisateur2Id == proprietaireId)
                .Select(c => c.Id)
                .ToListAsync();

            // 2. Compter les messages non lus dans ces conversations, envoyés par l'autre utilisateur
            var unreadMessagesCount = await _context.Messages
                .Where(m => conversationIds.Contains(m.ConversationId)
                            && m.EstLu == false
                            && m.ExpediteurId != proprietaireId)
                .CountAsync();

            // 3. Compter les demandes en attente
            var pendingDemandesCount = await _context.DemandesLocation
                .Where(d => d.Annonce.Logement.ProprietaireId == proprietaireId && d.status == LocationStatus.EnAttente)
                .CountAsync();

            return new OwnerStatsResponseDTO
            {
                UnreadMessages = unreadMessagesCount,
                PendingDemandes = pendingDemandesCount
            };
        }
    }
}
