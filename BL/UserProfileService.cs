using ColocationAppBackend.Data;
using ColocationAppBackend.DTOs.Responses;
using ColocationAppBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace ColocationAppBackend.BL
{
    public class UserProfileService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private string baseUrl;

        public UserProfileService(ApplicationDbContext context,IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
            this.baseUrl = _configuration["BaseUrl"];
        }

        public async Task<UserSummaryDto?> GetProfilUtilisateurAsync(int userId)
        {
            var utilisateur = await _context.Utilisateurs
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (utilisateur == null) return null;

            return new UserSummaryDto
            {
                Nom = utilisateur.Nom,
                Role = utilisateur.GetType().Name,
                AvatarUrl = $"{this.baseUrl}{utilisateur.AvatarUrl}"
               
            };
        }
    }
}
