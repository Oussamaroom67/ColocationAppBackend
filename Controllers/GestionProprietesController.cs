using ColocationAppBackend.BL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ColocationAppBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Administrateur")]
    public class GestionProprietesController : ControllerBase
    {
        private readonly GestionProprietes _gestionProprietes;
        public GestionProprietesController(GestionProprietes gestionProprietes)
        {
            _gestionProprietes = gestionProprietes;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllProprietes()
        {
            var result = await _gestionProprietes.getAllProprietes();
            return Ok(result);
        }
        [HttpPost]
        [Route("verify")]
        public async Task<IActionResult> Verify([FromQuery] int id)
        {
            var result = await _gestionProprietes.verifyProp(id);
            return Ok(result);
        }
        [HttpPost]
        [Route("rejete")]
        public async Task<IActionResult> Rejete([FromQuery] int id)
        {
            var result = await _gestionProprietes.rejeteProp(id);
            return Ok(result);
        }
    }
}
