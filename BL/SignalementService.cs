using ColocationAppBackend.Data;
using ColocationAppBackend.DTOs.Requests;
using ColocationAppBackend.Enums;
using ColocationAppBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace ColocationAppBackend.BL
{
    public class SignalementService
    {
        private readonly ApplicationDbContext _context;

        public SignalementService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AjouterSignalementAsync(SignalementRequest request)
        {
            // 1. Vérifier que l'annonce existe
            var annonce = await _context.Annonces
                .Include(a => a.Logement)
                .ThenInclude(l => l.Proprietaire)
                .FirstOrDefaultAsync(a => a.Id == request.AnnonceSignaleeId);

            if (annonce == null)
                throw new ArgumentException("L'annonce signalée n'existe pas.");

            // 2. Trouver le propriétaire de l'annonce
            var proprietaire = annonce.Logement?.Proprietaire;
            if (proprietaire == null)
                throw new ArgumentException("Impossible de trouver le propriétaire de cette annonce.");

            var signalement = new Signalement
            {
                SignaleurId = request.SignaleurId,
                AnnonceSignaleeId = request.AnnonceSignaleeId,
                UtilisateurSignaleId = proprietaire.Id,
                Motif = request.Motif,
                Description = request.Description,
                Statut = SignalementType.EnAttente,
                DateSignalement = DateTime.UtcNow,
                DateModification = DateTime.UtcNow
            };

            _context.Signalments.Add(signalement);
            await _context.SaveChangesAsync();
        }
    }
}
