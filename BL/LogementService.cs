using ColocationAppBackend.Data;
using ColocationAppBackend.DTOs.Responses;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace ColocationAppBackend.BL
{
    public class LogementService
    {
        private readonly ApplicationDbContext _context;
        public LogementService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<List<LogementDto>> GetAllLogementsAsync()
        {
            return await _context.Logements
            .Include(l => l.Proprietaire)
            .Select(l => new LogementDto
            {
                Id = l.Id,
                Titre = l.Annonce.Titre,
                Adresse = l.Adresse,
                ProprietaireNomComplet = l.Proprietaire.Nom + " " + l.Proprietaire.Prenom,
                Type = l.Type,
                Prix = l.Annonce.Prix.ToString(),
                Statut = l.Annonce.StatutVerification.ToString(),
                DateAjout = l.Annonce.DateModification
            }).ToListAsync();
        }
    }
}
