using ColocationAppBackend.Data;
using ColocationAppBackend.DTOs.Responses;
using Microsoft.EntityFrameworkCore;


namespace ColocationAppBackend.BL
{
    public class GestionSignalements
    {
        private readonly ApplicationDbContext _context;
        public GestionSignalements(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<List<GestionSignalementsDto>> getAllSignalements()
        {
            return await _context.Signalments.Select(u => new GestionSignalementsDto
            {
                id= u.Id,
                status = u.Statut.ToString(),
                DateSignalement = u.DateSignalement.ToString("yyyy-MM-dd"),
                DateResolution = u.DateResolution.ToString("yyyy-MM-dd"),
                signaleurName = u.Signaleur.Nom + " " + u.Signaleur.Prenom,
                signaleurEmail = u.Signaleur.Email,
                description = u.Description,
                contentType = u.AnnonceSignalee.Logement.Type.ToString(),
                UtilisateurSignaleName = u.UtilisateurSignale.Nom + " " + u.UtilisateurSignale.Prenom,
                contentName = u.AnnonceSignalee.Titre,
                contentId = u.AnnonceSignaleeId,
                motif = u.Motif
            }).ToListAsync();
        } 
        public async Task<bool> resoudre(int id)
        {
            var signalement = await _context.Signalments.FirstOrDefaultAsync(a => a.Id == id);
            if (signalement == null) return false;
            signalement.Statut = Enums.SignalementType.Resolu;
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> rejeter(int id)
        {
            var signalement = await _context.Signalments.FirstOrDefaultAsync(a => a.Id == id);
            if (signalement == null) return false;
            signalement.Statut = Enums.SignalementType.Rejete;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
