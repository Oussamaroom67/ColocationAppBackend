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
    }
}
