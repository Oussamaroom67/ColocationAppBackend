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


        [Authorize(Roles = "Etudiant")]
        [HttpPost("postuler")]
        public async Task<IActionResult> PostulerColocation([FromBody] PostulerColocationRequest request)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int etudiantId))
                {
                    return Unauthorized(new { error = "Utilisateur non identifié" });
                }

                var result = await _manager.PostulerColocationAsync(request, etudiantId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // 3. Obtenir mes demandes envoyées
        [Authorize(Roles = "Etudiant")]
        [HttpGet("mes-demandes")]
        public async Task<IActionResult> ObtenirMesDemandes()
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int etudiantId))
                {
                    return Unauthorized(new { error = "Utilisateur non identifié" });
                }

                var result = await _manager.ObtenirMesDemandesAsync(etudiantId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // 4. Annuler une demande
        [Authorize(Roles = "Etudiant")]
        [HttpDelete("annuler-demande")]
        public async Task<IActionResult> AnnulerDemande([FromBody] AnnulerDemandeRequest request)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int etudiantId))
                {
                    return Unauthorized(new { error = "Utilisateur non identifié" });
                }

                var result = await _manager.AnnulerDemandeAsync(request.DemandeId, etudiantId);
                return Ok(new { message = "Demande annulée avec succès", success = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // 5. Obtenir les demandes reçues (pour le propriétaire)
        [Authorize(Roles = "Etudiant")]
        [HttpGet("demandes-recues")]
        public async Task<IActionResult> ObtenirDemandesRecues()
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int proprietaireId))
                {
                    return Unauthorized(new { error = "Utilisateur non identifié" });
                }

                var result = await _manager.ObtenirDemandesRecuesAsync(proprietaireId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // 6. Répondre à une demande (accepter/refuser)
        [Authorize(Roles = "Etudiant")]
        [HttpPost("repondre-demande")]
        public async Task<IActionResult> RepondreDemande([FromBody] RepondreDemandeRequest request)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int proprietaireId))
                {
                    return Unauthorized(new { error = "Utilisateur non identifié" });
                }

                var result = await _manager.RepondreDemandeAsync(request, proprietaireId);
                return Ok(new { message = "Réponse envoyée avec succès", success = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}