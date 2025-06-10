using ColocationAppBackend.BL;
using ColocationAppBackend.DTOs.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ColocationAppBackend.Controllers
{
    [ApiController]
    [Route("api/Colocation")]
    public class ColocationController : ControllerBase
    {
        private readonly ColocationManager _manager;

        public ColocationController(ColocationManager manager)
        {
            _manager = manager;
        }

        [Authorize(Roles = "Etudiant")]
        [HttpPost("create")]
        public async Task<IActionResult> AjouterColocation([FromBody] AjouterColocationRequest request)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int etudiantId))
                {
                    return Unauthorized(new { error = "Utilisateur non identifié" });
                }

                var result = await _manager.AjouterColocationAsync(request, etudiantId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}