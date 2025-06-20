using ColocationAppBackend.Data;
using ColocationAppBackend.Enums;
using ColocationAppBackend.Models;
using Microsoft.EntityFrameworkCore;


namespace ColocationAppBackend.BL
{
    public class FavoriService
    {
        private readonly ApplicationDbContext _context;

        public FavoriService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AjouterFavoriAsync(int etudiantId, int elementId, TypeFavori type)
        {
            var existe = await _context.Favoris
                .AnyAsync(f => f.EtudiantId == etudiantId &&
                              ((type == TypeFavori.Propriete && f.AnnonceId == elementId) ||
                               (type == TypeFavori.Colocation && f.OffreColocationId == elementId)));

            if (existe)
            {
                return;
            }
            var favori = new Favori
            {
                EtudiantId = etudiantId,
                Type = type,
                AnnonceId = type == TypeFavori.Propriete ? elementId : null,
                OffreColocationId = type == TypeFavori.Colocation ? elementId : null
            };
            _context.Favoris.Add(favori);
            await _context.SaveChangesAsync();
        }

        public async Task SupprimerFavoriAsync(int etudiantId, int elementId, TypeFavori type)
        {
            var favori = await _context.Favoris.FirstOrDefaultAsync(f =>
                f.EtudiantId == etudiantId &&
                ((type == TypeFavori.Propriete && f.AnnonceId == elementId) ||
                 (type == TypeFavori.Colocation && f.OffreColocationId == elementId)));

            if (favori != null)
            {
                _context.Favoris.Remove(favori);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<Annonce>> ObtenirFavorisAnnoncesAsync(int etudiantId)
        {
            var annonceIds = await _context.Favoris
                .Where(f => f.EtudiantId == etudiantId && f.Type == TypeFavori.Propriete)
                .Select(f => f.AnnonceId)
                .ToListAsync();

            return await _context.Annonces
                .Where(a => annonceIds.Contains(a.Id))
                .ToListAsync();
        }

        public async Task<List<Colocation>> ObtenirFavorisColocationsAsync(int etudiantId)
        {
            var colocIds = await _context.Favoris
                .Where(f => f.EtudiantId == etudiantId && f.Type == TypeFavori.Colocation)
                .Select(f => f.OffreColocationId)
                .ToListAsync();

            return await _context.Colocations
                .Where(c => colocIds.Contains(c.Id))
                .ToListAsync();
        }
    }
}
