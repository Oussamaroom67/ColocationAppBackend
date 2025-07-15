using ColocationAppBackend.Data;
using ColocationAppBackend.DTOs.Responses;
using Microsoft.EntityFrameworkCore;
namespace ColocationAppBackend.BL
{
    public class AboutUsStatsService
    {
        private readonly ApplicationDbContext _context;
        public AboutUsStatsService(ApplicationDbContext context) {
            _context = context;
        }
        public async Task<AboutUsStatsDto> GetStats()
        {
            var aboutUsStatsDto = new AboutUsStatsDto
            {
                OwnerCount = await _context.Proprietaires.CountAsync(),
                StudentCount = await _context.Etudiants.CountAsync(),
                ColocationCount = await _context.DemandesColocation.CountAsync(),
                CityCount = await _context.Proprietaires
               .Select(p => p.Ville) 
               .Distinct()
               .CountAsync()
                };

            return aboutUsStatsDto;

        }

    }
}
