using ColocationAppBackend.BL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ColocationAppBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Proprietaire")] 
    public class AnnonceModificationController : ControllerBase
    {
        private readonly AnnonceModificationService _annonceService;

        public AnnonceModificationController(AnnonceModificationService annonceService)
        {
            _annonceService = annonceService;
        }

        // GET: api/AnnonceModification/mes-annonces
        [HttpGet("mes-annonces")]
        public async Task<IActionResult> GetMesAnnonces()
        {
            // Extraction de l'id utilisateur depuis les claims
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized("Utilisateur non authentifié.");

            if (!int.TryParse(userIdClaim.Value, out int proprietaireId))
                return BadRequest("Identifiant utilisateur invalide.");

            var annonces = await _annonceService.GetAnnoncesByProprietaireAsync(proprietaireId);
            return Ok(annonces);
        }

        // GET: api/AnnonceModification/{annonceId}
        [HttpGet("{annonceId}")]
        public async Task<IActionResult> GetAnnonceById(int annonceId)
        {
            var annonce = await _annonceService.GetAnnonceCompleteAsync(annonceId);
            if (annonce == null)
                return NotFound("Annonce introuvable.");

            return Ok(annonce);
        }
    }
}
