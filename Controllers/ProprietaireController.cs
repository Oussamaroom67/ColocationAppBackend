using ColocationAppBackend.BL;
using ColocationAppBackend.DTOs.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ColocationAppBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProprietaireController : ControllerBase
    {
        private readonly ProprietaireManager _manager;
        public ProprietaireController(ProprietaireManager manager)
        {
            _manager = manager;
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] LogementAnnonceRequest req)
        {
            try
            {
                var result = await _manager.AjouterLogementEtAnnonceAsync(req);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [Authorize(Roles = "Proprietaire")]
        [HttpGet("annonces")]
        public async Task<IActionResult> GetAnnonces()
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int proprietaireId))
                {
                    return Unauthorized(new { error = "Utilisateur non identifié" });
                }

                var annonces = await _manager.GetAnnoncesByProprietaireIdAsync(proprietaireId);
                return Ok(annonces);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}