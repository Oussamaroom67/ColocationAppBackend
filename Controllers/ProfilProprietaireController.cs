using ColocationAppBackend.BL;
using ColocationAppBackend.DTOs.Requests;
using ColocationAppBackend.DTOs.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ColocationAppBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Proprietaire")]
    public class ProfilProprietaireController : ControllerBase
    {
        private readonly ProfilProprietaireService _service;

        public ProfilProprietaireController(ProfilProprietaireService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetProfil()
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int proprietaireId))
                {
                    return Unauthorized(new { error = "Utilisateur non identifié" });
                }

                var profil = await _service.GetProfilAsync(proprietaireId);
                return Ok(profil);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateProfil([FromBody] UpdateProfilProprietaireRequest request)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int proprietaireId))
                {
                    return Unauthorized(new { error = "Utilisateur non identifié" });
                }

                var profil = await _service.UpdateProfilAsync(proprietaireId, request);
                return Ok(new
                {
                    message = "Profil mis à jour avec succès",
                    profil = profil
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

    }
}