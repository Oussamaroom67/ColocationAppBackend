using ColocationAppBackend.BL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ColocationAppBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles="Administrateur")]
    public class GestionSignalementsController : ControllerBase
    {
        private readonly GestionSignalements _gestionSignalements;
        public GestionSignalementsController(GestionSignalements gestionSignalements)
        {
            _gestionSignalements = gestionSignalements;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllSignalements()
        {
            var data = await _gestionSignalements.getAllSignalements();
            return Ok(data);
        }
        [HttpPost]
        [Route("Resout")]
        public async Task<IActionResult> Resoudre([FromQuery] int id)
        {
            var data = await _gestionSignalements.resoudre(id);
            return Ok(data);
        }
        [HttpPost]
        [Route("Rejete")]
        public async Task<IActionResult> Rejeter([FromQuery] int id)
        {
            var data = await _gestionSignalements.rejeter(id);
            return Ok(data);
        }
    }
}
