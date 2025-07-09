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
            try
            {
                var isDeleted = await _utilisateurService.DeleteUser(idUser);
                if (!isDeleted)
                    return NotFound("Utilisateur introuvable ou suppression impossible.");

                return Ok("Utilisateur supprimé avec succès.");
            }
            catch (Exception ex)
            {
                var realMessage = ex.InnerException?.InnerException?.Message ?? ex.InnerException?.Message ?? ex.Message;
                return BadRequest(new
                {

                    message = "Erreur lors de la suppression de l'utilisateur.",
                    details = realMessage,
                    innerException = ex.InnerException?.Message
                });
            }
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
