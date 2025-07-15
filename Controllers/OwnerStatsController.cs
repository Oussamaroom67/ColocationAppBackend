using ColocationAppBackend.BL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ColocationAppBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Proprietaire")] 
    public class OwnerStatsController : ControllerBase
    {
        private readonly OwnerStatsService _statsService;

        public OwnerStatsController(OwnerStatsService statsService)
        {
            _statsService = statsService;
        }

        [HttpGet]
        public async Task<IActionResult> GetOwnerStats()
        {
            // Récupérer l’ID utilisateur depuis le token
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
                return Unauthorized("Token invalide");

            int proprietaireId = int.Parse(userIdClaim.Value);

            var stats = await _statsService.GetStatsAsync(proprietaireId);
            return Ok(stats);
        }
    }
}
