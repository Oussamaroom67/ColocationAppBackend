using ColocationAppBackend.BL;
using ColocationAppBackend.DTOs.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ColocationAppBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProprietaireController : ControllerBase
    {
        private readonly ProprietaireManager _manager;
        public ProprietaireController(ProprietaireManager manager)
        {
            _manager = manager;
        }

        [HttpPost("save")]
        public async Task<IActionResult> Create([FromBody] LogementAnnonceRequest req)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int proprietaireId))
                {
                    return Unauthorized(new { error = "Utilisateur non identifié" });
                }

                // Associer le propriétaire au logement
                req.ProprietaireId = proprietaireId;

                var result = await _manager.AjouterLogementEtAnnonceAsync(req);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [Authorize(Roles = "Proprietaire")]
        [HttpGet("annonces")]
        public async Task<IActionResult> GetAnnonces()
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int proprietaireId))
                {
                    return Unauthorized(new { error = "Utilisateur non identifié" });
                }

                var annonces = await _manager.GetAnnoncesByProprietaireIdAsync(proprietaireId);
                return Ok(annonces);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [Authorize(Roles = "Proprietaire")]
        [HttpDelete("supprimer")]
        public async Task<IActionResult> SupprimerLogement([FromBody] SupprimerLogementRequest request)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int proprietaireId))
                {
                    return Unauthorized(new { error = "Utilisateur non identifié" });
                }

                var result = await _manager.SupprimerLogementAsync(request.LogementId, proprietaireId);
                return Ok(new { message = "Logement supprimé avec succès" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
        [Authorize(Roles = "Proprietaire")]
        [HttpPut("changer-statut")]

        public async Task<IActionResult> ChangerStatutAnnonce([FromBody] ChangerStatutAnnonceRequest request)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int proprietaireId))
                {
                    return Unauthorized(new { error = "Utilisateur non identifié" });
                }

                var result = await _manager.ChangerStatutAnnonceAsync(request.AnnonceId, request.NouveauStatut, proprietaireId);
                return Ok(new { message = "Statut de l'annonce modifié avec succès" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}