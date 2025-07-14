using ColocationAppBackend.BL;
using ColocationAppBackend.DTOs.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ColocationAppBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserProfileController : ControllerBase
    {
        private readonly UserProfileService _profilService;

        public UserProfileController(UserProfileService profilService)
        {
            _profilService = profilService;
        }

        [HttpGet("resume")]
        [Authorize]
        public async Task<ActionResult<UserSummaryDto>> GetProfilUtilisateur()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userIdClaim == null || !int.TryParse(userIdClaim, out int userId))
                return Unauthorized("Utilisateur non connecté");

            var profil = await _profilService.GetProfilUtilisateurAsync(userId);

            if (profil == null)
                return NotFound("Utilisateur non trouvé");

            return Ok(profil);
        }
    }
}
