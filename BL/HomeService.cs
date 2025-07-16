using ColocationAppBackend.Data;
using ColocationAppBackend.DTOs.Responses;
using Microsoft.EntityFrameworkCore;
using static System.Net.Mime.MediaTypeNames;

namespace ColocationAppBackend.BL
{
    public class HomeService
    {
        private readonly ApplicationDbContext _context;
        public HomeService(ApplicationDbContext context)
        {
            _context = context;
        }
        //statistics
        public async Task<HomeStatisticDTO> GetStatistic()
        {
            return (new HomeStatisticDTO
            {
                totalStudents = await _context.Etudiants.CountAsync(),
                totalProperties = await _context.Annonces.CountAsync(),
                totalUniversities = await _context.Etudiants.Select(e=>e.Universite).Distinct().CountAsync(),
                roomateMatches = await _context.DemandesColocation.CountAsync()
            });
        }
        //Featured Properties
        public async Task<List<FeaturedPropDto>> GetFeatured()
        {
            return await _context.Annonces
                .Include(a => a.Photos)
                .Include(a=>a.Logement)
                .Select(a => new FeaturedPropDto
            {
                Id = a.Id,
                Image = a.Photos.FirstOrDefault().Url,
                Titre = a.Titre,
                Adresse = a.Logement.Adresse,
                NbSallesBain= a.Logement.NbSallesBain,
                NbChambres = a.Logement.NbChambres,
                prix = a.Prix
            })
            .Take(3)
            .ToListAsync();
        }
    }
}
