using Microsoft.AspNetCore.Mvc;
using ColocationAppBackend.DTOs.Responses;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using ColocationAppBackend.BL;
using ColocationAppBackend.DTOs.Requests;

namespace ColocationAppBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DemandesLocationController : ControllerBase
    {
        private readonly DemandeLocationService _demandeLocationService;

        public DemandesLocationController(DemandeLocationService demandeLocationService)
        {
            _demandeLocationService = demandeLocationService;
        }

        [HttpPost("envoyer")]
        public async Task<ActionResult<DemandeLocationResponseDto>> EnvoyerDemande([FromBody] DemandeLocationCreateDto demandeDto)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int etudiantId))
                {
                    return Unauthorized(new { error = "Utilisateur non identifié" });
                }

                // Assigner l'ID de l'étudiant depuis le token
                demandeDto.EtudiantId = etudiantId;

                var result = await _demandeLocationService.EnvoyerDemandeAsync(demandeDto);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erreur interne du serveur" });
            }
        }

        [HttpPut("changer-status")]
        public async Task<ActionResult<DemandeLocationResponseDto>> ChangerStatus([FromBody] ChangeStatusDto changeStatusDto)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int proprietaireId))
                {
                    return Unauthorized(new { error = "Utilisateur non identifié" });
                }

                var result = await _demandeLocationService.ChangerStatusAsync(changeStatusDto, proprietaireId);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erreur interne du serveur" });
            }
        }

        [HttpGet("proprietaire/demandes-recues")]
        public async Task<ActionResult<List<DemandeLocationRecusProprietaireResponseDto>>> GetAllReceivedRequestsLocation()
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int proprietaireId))
                {
                    return Unauthorized(new { error = "Utilisateur non identifié" });
                }

                var demandes = await _demandeLocationService.GetAllReceivedRequestsLocation(proprietaireId);
                return Ok(demandes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erreur interne du serveur" });
            }
        }

        [HttpGet("mes-demandes")]
        public async Task<ActionResult<List<DemandeLocationResponseDto>>> GetMesDemandesEtudiant()
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int etudiantId))
                {
                    return Unauthorized(new { error = "Utilisateur non identifié" });
                }

                var demandes = await _demandeLocationService.GetDemandesParEtudiantAsync(etudiantId);
                return Ok(demandes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erreur interne du serveur" });
            }
        }
    }
}
