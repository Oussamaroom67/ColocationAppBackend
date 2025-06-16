using ColocationAppBackend.BL;
using ColocationAppBackend.DTOs.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ColocationAppBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class SignalementController : ControllerBase
    {
        private readonly SignalementService _signalementService;

        public SignalementController(SignalementService signalementService)
        {
            _signalementService = signalementService;
        }

        private int GetUserId()
        {
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(idClaim, out var id) ? id : throw new UnauthorizedAccessException("Identifiant utilisateur invalide.");
        }

        [HttpPost("signaler")]
        public async Task<IActionResult> Signaler([FromBody] SignalementRequest request)
        {
            try
            {
                request.SignaleurId = GetUserId();
                await _signalementService.AjouterSignalementAsync(request);
                return Ok(new { message = "Signalement effectué avec succès." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

    }
}
