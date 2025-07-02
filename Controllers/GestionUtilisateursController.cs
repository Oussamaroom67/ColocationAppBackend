using ColocationAppBackend.BL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ColocationAppBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GestionUtilisateursController : ControllerBase
    {
        private readonly GestionUtilisateurs _GestionUtilisateurs;
        public GestionUtilisateursController(GestionUtilisateurs gestionUtilisateurs)
        {
            _GestionUtilisateurs = gestionUtilisateurs;
        }
        [Authorize(Roles ="Administrateur")]
        [HttpGet("GetAllUsers")]
        public async Task<IActionResult> getAllStudent()
        {
            var result = await _GestionUtilisateurs.GetAllUsers();
            return Ok(result);
        } 
    }
}
