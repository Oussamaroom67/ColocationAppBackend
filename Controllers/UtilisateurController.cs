using ColocationAppBackend.BL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ColocationAppBackend.Controllers
{
    [Authorize(Roles = "Administrateur")]
    [ApiController]
    [Route("api/[controller]")]
    public class UtilisateurController: ControllerBase
    {
        private readonly UtilisateurService _utilisateurService;


        public UtilisateurController(UtilisateurService utilisateurService)
        {
            _utilisateurService = utilisateurService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var utilisateurs = await _utilisateurService.GetAllUtilisateursAsync();
            return Ok(utilisateurs);
        }
        [HttpPost]
        [Route("DeleteUser")]
        public async Task<IActionResult> DeleteUser([FromQuery] int idUser)
        {
            var IsDeleted = await _utilisateurService.DeleteUser(idUser);
            return Ok(IsDeleted);
        }

        [HttpPost]
        [Route("SuspendreUser")]
        public async Task<IActionResult> SuspendreUser([FromQuery] int idUser,[FromQuery] bool suspendre)
        {
            var result = await _utilisateurService.SuspendreUser(idUser, suspendre);
            if (!result) return NotFound("Utilisateur non trouvé");

            return Ok(new { success = true, estSuspendu = suspendre });
        }
    }
}
