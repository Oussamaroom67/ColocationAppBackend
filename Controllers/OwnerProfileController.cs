using ColocationAppBackend.BL;
using ColocationAppBackend.DTOs.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ColocationAppBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OwnerProfileController : ControllerBase
    {
        private readonly OwnerProfile _ownerProfile;
        public OwnerProfileController(OwnerProfile ownerProfile)
        {
            _ownerProfile = ownerProfile;
        }
        [HttpGet("info")]
        public async Task<IActionResult> GetInfo([FromQuery] int id)
        {
            var result = await _ownerProfile.getInfo(id);
            return Ok(result);
        }
        [HttpPost("AddAvis")]
        [Authorize(Roles = "Etudiant")]
        public async Task<IActionResult> AddAvis([FromBody] AvisDto avis)
        {
            await _ownerProfile.AddAvis(avis);
            return Ok();
        }

    }
}
