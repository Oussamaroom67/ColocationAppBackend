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
        private readonly IConfiguration _configuration;
        private string baseUrl;

        public AnnonceFilterService(ApplicationDbContext context,IConfiguration configuration)
        {
            _context = context;
            _configuration=configuration;
            baseUrl = _configuration["BaseUrl"];
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

            if (filter.MinPrice.HasValue && filter.MinPrice.Value > 0)
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
                Photos = a.Photos.Select(p => new PhotoDto { Url = $"{baseUrl}{p.Url}" }).ToList()
            }).ToList();
        }

        public List<AnnonceResponse> FilterAdvanced(AdvancedFilterDTO filter)
        {
            var query = _context.Annonces
                .Include(a => a.Logement)
                .Include(a => a.Photos)
                .AsQueryable();

            if (filter.Price.HasValue)
                query = query.Where(a => a.Prix <= filter.Price);

            if (filter.PropertyType != null && filter.PropertyType.Any())
                query = query.Where(a => filter.PropertyType.Contains(a.Logement.Type));

            if (filter.Bedrooms != null)
                query = filter.Bedrooms switch
                {
                    "1" => query.Where(a => a.Logement.NbChambres == 1),
                    "2" => query.Where(a => a.Logement.NbChambres == 2),
                    "3" => query.Where(a => a.Logement.NbChambres == 3),
                    "4+" => query.Where(a => a.Logement.NbChambres >= 4),
                    "Any" => query,
                    _ => query
                };

            if (filter.Bathrooms != null)
            {
                query = filter.Bathrooms switch
                {
                    "1" => query.Where(a => a.Logement.NbSallesBain == 1),
                    "2" => query.Where(a => a.Logement.NbSallesBain == 2),
                    "3" => query.Where(a => a.Logement.NbSallesBain == 3),
                    "4+" => query.Where(a => a.Logement.NbSallesBain >= 4),
                    "Any" => query,
                    _ => query
                };
            }
                
            if (filter.Amenities != null && filter.Amenities.Any())
            {
                foreach (var amenity in filter.Amenities)
                {
                    switch (amenity.ToLower())
                    {
                        case "parking":
                            query = query.Where(a => a.Logement.ParkingDisponible == true);
                            break;
                        case "Wi-Fi inclus":
                            query = query.Where(a => a.Logement.InternetInclus == true);
                            break;
                        case "Animaux acceptés":
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
                Photos = a.Photos.Select(p => new PhotoDto { Url = $"{baseUrl}{p.Url}" }).ToList()
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
                    Photos = a.Photos.Select(p => new PhotoDto { Url = $"{baseUrl}{p.Url}" }).ToList()
                }).ToList();
        }
    }
}
