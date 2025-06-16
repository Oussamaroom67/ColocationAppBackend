using ColocationAppBackend.Data;
using ColocationAppBackend.DTOs.Requests;
using ColocationAppBackend.DTOs.Responses;
using ColocationAppBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace ColocationAppBackend.BL
{
    public class AnnonceFilterService
    {
        private readonly ApplicationDbContext _context;

        public AnnonceFilterService(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<AnnonceResponse> FilterBasic(BasicFilterDTO filter)
        {
            var query = _context.Annonces
                .Include(a => a.Logement)
                .Include(a => a.Photos)
                .AsQueryable();

            if (!string.IsNullOrEmpty(filter.Ville))
                query = query.Where(a => a.Logement.Ville == filter.Ville);

            if (!string.IsNullOrEmpty(filter.PropertyType))
                query = query.Where(a => a.Logement.Type == filter.PropertyType);

            if (filter.MinPrice.HasValue)
                query = query.Where(a => a.Prix <= filter.MinPrice.Value);

            return query.Select(a => new AnnonceResponse
            {
                AnnonceId = a.Id,
                LogementId = a.LogementId,
                Title = a.Titre,
                Type = a.Logement.Type,
                Prix = a.Prix,
                Ville = a.Logement.Ville,
                Beds = a.Logement.NbChambres,
                Baths = a.Logement.NbSallesBain,
                Photos = a.Photos.Select(p => new PhotoDto { Url = p.Url }).ToList()
            }).ToList();
        }

        public List<AnnonceResponse> FilterAdvanced(AdvancedFilterDTO filter)
        {
            var query = _context.Annonces
                .Include(a => a.Logement)
                .Include(a => a.Photos)
                .AsQueryable();

            if (filter.PropertyType != null && filter.PropertyType.Any())
                query = query.Where(a => filter.PropertyType.Contains(a.Logement.Type));

            if (filter.Bedrooms.HasValue)
                query = query.Where(a => a.Logement.NbChambres == filter.Bedrooms.Value);

            if (filter.Bathrooms.HasValue)
                query = query.Where(a => a.Logement.NbSallesBain == filter.Bathrooms.Value);

            if (filter.Amenities != null && filter.Amenities.Any())
            {
                foreach (var amenity in filter.Amenities)
                {
                    switch (amenity.ToLower())
                    {
                        case "parking":
                            query = query.Where(a => a.Logement.ParkingDisponible == true);
                            break;
                        case "wifi included":
                            query = query.Where(a => a.Logement.InternetInclus == true);
                            break;
                        case "pet friendly":
                            query = query.Where(a => a.Logement.AnimauxAutorises == true);
                            break;
                    }
                }
            }

            return query.Select(a => new AnnonceResponse
            {
                AnnonceId = a.Id,
                LogementId = a.LogementId,
                Title = a.Titre,
                Type = a.Logement.Type,
                Prix = a.Prix,
                Ville = a.Logement.Ville,
                Beds = a.Logement.NbChambres,
                Baths = a.Logement.NbSallesBain,
                Photos = a.Photos.Select(p => new PhotoDto { Url = p.Url }).ToList()
            }).ToList();
        }

        public List<AnnonceResponse> GetAllAnnonces()
        {
            return _context.Annonces
                .Include(a => a.Logement)
                .Include(a => a.Photos)
                .Select(a => new AnnonceResponse
                {
                    AnnonceId = a.Id,
                    LogementId = a.LogementId,
                    Title = a.Titre,
                    Type = a.Logement.Type,
                    Prix = a.Prix,
                    Ville = a.Logement.Ville,
                    Beds = a.Logement.NbChambres,
                    Baths = a.Logement.NbSallesBain,
                    Photos = a.Photos.Select(p => new PhotoDto { Url = p.Url }).ToList()
                }).ToList();
        }
    }
}
