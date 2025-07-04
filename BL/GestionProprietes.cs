using ColocationAppBackend.Data;
using ColocationAppBackend.DTOs.Responses;
using ColocationAppBackend.Enums;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace ColocationAppBackend.BL
{
    public class GestionProprietes
    {
        private readonly ApplicationDbContext _context;
        public GestionProprietes(ApplicationDbContext context)
        {
            _context = context;
        }
        //getAllProprietes
        public async Task<List<GestionProprietesDto>> getAllProprietes()
        {
            return await _context.Logements.Select(u=> new GestionProprietesDto { 
                id = u.Id,
                nom = u.Annonce.Titre,
                adresse = u.Adresse,
                nomProprietaire = u.Proprietaire.Nom + " " + u.Proprietaire.Prenom,
                type = u.Type.ToString(),
                prix = u.Annonce.Prix,
                statut = u.status.ToString(),
                dateAjout = u.Annonce.DateModification.ToString("yyyy-MM-dd"),
            }).ToListAsync();
        }
        //verifiée
        public async Task<bool> verifyProp(int id)
        {
            var logement = await  _context.Logements.FirstOrDefaultAsync(u => u.Id == id);
            if (logement == null) return false;
            logement.status = LogementStatus.Verifié;
            await _context.SaveChangesAsync();
            return true;
        }
        //rejeté
        public async Task<bool> rejeteProp(int id)
        {
            var logement = await _context.Logements.FirstOrDefaultAsync(u => u.Id == id);
            if (logement == null) return false;
            logement.status = LogementStatus.Rejeté;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
