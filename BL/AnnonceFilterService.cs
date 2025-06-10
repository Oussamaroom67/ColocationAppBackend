using ColocationAppBackend.Data;
using ColocationAppBackend.DTOs.Requests;
using ColocationAppBackend.Models;

namespace ColocationAppBackend.BL
{
    public class AnnonceFilterService
    {
        private readonly ApplicationDbContext _context;
        public AnnonceFilterService(ApplicationDbContext context)
        {
            _context = context;
        }
        public List<Annonce> FilterBasic(BasicFilterDTO filter)
        {
            var query = _context.Annonces.AsQueryable();

            if (!string.IsNullOrEmpty(filter.Ville))
                query = query.Where(a => a.Logement.Ville == filter.Ville);

            if (!string.IsNullOrEmpty(filter.PropertyType))
                query = query.Where(a => a.Logement.Type == filter.PropertyType);

            if (filter.MinPrice.HasValue)
                query = query.Where(a => a.Prix <= filter.MinPrice.Value);

            return query.ToList();
        }

        public List<Annonce> FilterAdvanced(AdvancedFilterDTO filter)
        {
            var query = _context.Annonces.AsQueryable();

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
                        case "Parking":
                            query = query.Where(a => a.Logement.ParkingDisponible == true);
                            break;
                        case "WiFi included":
                            query = query.Where(a => a.Logement.InternetInclus == true);
                            break;
                        case "Pet Friendly":
                            query = query.Where(a => a.Logement.AnimauxAutorises == true);
                            break;
                    }
                }
            }
            return query.ToList();
        }
    }
}
