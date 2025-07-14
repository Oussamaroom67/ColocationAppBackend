using ColocationAppBackend.Data;
using ColocationAppBackend.Enums;
using ColocationAppBackend.Models;
using Microsoft.EntityFrameworkCore;
using ColocationAppBackend.DTOs.Responses;
using ColocationAppBackend.DTOs.Requests;


namespace ColocationAppBackend.BL
{
    public class FavoriService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private string baseUrl;



        public FavoriService(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
            this.baseUrl = _configuration["BaseUrl"];
        }

        public async Task AjouterFavoriAsync(int etudiantId, int elementId, TypeFavori type)
        {
            var existe = await _context.Favoris
                .Where(f=>f.Annonce.Statut==AnnonceStatus.Active)
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

        public async Task<List<FavoriResponse>> ObtenirFavorisAnnoncesAsync(int etudiantId)
        {
            var favoris = await _context.Favoris
                .Include(f => f.Annonce)
                    .ThenInclude(a => a.Logement)
                .Include(f => f.Annonce)
                    .ThenInclude(a => a.Photos)
                .Where(f => f.EtudiantId == etudiantId && f.Type == TypeFavori.Propriete)
                .Select(f => new FavoriResponse
                {
                    Id = f.Annonce.Id,
                    Titre = f.Annonce.Titre,
                    Prix = f.Annonce.Prix,
                    Type = f.Annonce.Logement.Type,
                    Location = f.Annonce.Logement.Ville + ", " + f.Annonce.Logement.Adresse,
                    Chambres = f.Annonce.Logement.NbChambres,
                    Bathrooms = f.Annonce.Logement.NbSallesBain,
                    State = f.Annonce.Statut.ToString(),
                    Image = f.Annonce.Photos.Select(p => $"{baseUrl}{p.Url}").FirstOrDefault() ?? string.Empty

                })
                .ToListAsync();

            return favoris;
        }

        public async Task<List<FavoriColocationResponse>> ObtenirFavorisColocationsAsync(int etudiantId)
        {
            var favoris = await _context.Favoris
                .Include(f => f.OffreColocation)
                    .ThenInclude(c => c.Etudiant)
                .Where(f => f.EtudiantId == etudiantId && f.Type == TypeFavori.Colocation)
                .Select(f => new FavoriColocationResponse
                {
                    Id = f.OffreColocation.Id,
                    Name = f.OffreColocation.Etudiant.Nom + " " + f.OffreColocation.Etudiant.Prenom,
                    School = f.OffreColocation.Etudiant.Universite,
                    Budget = f.OffreColocation.Budget,
                    MoveInDate = f.OffreColocation.DateDebutDisponibilite.ToString("dd/MM/yyyy"),
                    PreferredZone = f.OffreColocation.Adresse
                })
                .ToListAsync();

            return favoris;
        }
    }
}
