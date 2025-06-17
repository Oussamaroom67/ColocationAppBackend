// Controllers/DashboardController.cs
using ColocationAppBackend.BL;
using ColocationAppBackend.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using ColocationAppBackend.DTOs.Responses;

namespace ColocationAppBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DashboardStudentController : ControllerBase
    {
        private readonly DashboardStudentService _dashboardStudentService;

        public DashboardStudentController(DashboardStudentService dashboardStudentService)
        {
            _dashboardStudentService = dashboardStudentService;
        }

        /// <summary>
        /// Récupère les statistiques du dashboard pour l'étudiant connecté
        /// </summary>
        /// <returns>Statistiques du dashboard</returns>
        [HttpGet("stats")]
        public async Task<ActionResult<DashboardStudentDto>> GetDashboardStats()
        {
            try
            {
                // Récupérer l'ID de l'étudiant depuis le token
                var etudiantIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(etudiantIdClaim) || !int.TryParse(etudiantIdClaim, out int etudiantId))
                {
                    return Unauthorized("Token invalide");
                }

                var stats = await _dashboardStudentService.GetDashboardStatsAsync(etudiantId);
                return Ok(stats);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Une erreur est survenue lors de la récupération des statistiques");
            }
        }

        /// <summary>
        /// Récupère les 3 derniers messages reçus par l'étudiant connecté
        /// </summary>
        /// <returns>Liste des 3 derniers messages</returns>
        [HttpGet("recent-messages")]
        public async Task<ActionResult<List<MessageRecentDto>>> GetRecentMessages()
        {
            try
            {
                // Récupérer l'ID de l'étudiant depuis le token
                var etudiantIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(etudiantIdClaim) || !int.TryParse(etudiantIdClaim, out int etudiantId))
                {
                    return Unauthorized("Token invalide");
                }

                var messages = await _dashboardStudentService.GetRecentMessagesAsync(etudiantId);
                return Ok(messages);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Une erreur est survenue lors de la récupération des messages");
            }
        }

        /// <summary>
        /// Récupère les 3 dernières annonces publiées récemment
        /// </summary>
        /// <returns>Liste des 3 dernières annonces</returns>
        [HttpGet("recent-properties")]
        public async Task<ActionResult<List<PropertyRecentDto>>> GetRecentProperties()
        {
            try
            {
                // Récupérer l'ID de l'étudiant depuis le token
                var etudiantIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(etudiantIdClaim) || !int.TryParse(etudiantIdClaim, out int etudiantId))
                {
                    return Unauthorized("Token invalide");
                }

                var properties = await _dashboardStudentService.GetRecentPropertiesAsync(etudiantId);
                return Ok(properties);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Une erreur est survenue lors de la récupération des propriétés");
            }
        }
        /// <summary>
        /// Récupère toutes les demandes de colocations reçues sur les colocations de l'étudiant connecté
        /// </summary>
        /// <returns>Liste des demandes de colocations</returns>
        [HttpGet("demandes-colocations")]
        public async Task<ActionResult<List<DemandeColocationRecuDto>>> GetDemandesColocations()
        {
            try
            {
                // Récupérer l'ID de l'étudiant depuis le token
                var etudiantIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(etudiantIdClaim) || !int.TryParse(etudiantIdClaim, out int etudiantId))
                {
                    return Unauthorized("Token invalide");
                }

                var demandes = await _dashboardStudentService.GetDemandesColocationAsync(etudiantId);
                return Ok(demandes);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Une erreur est survenue lors de la récupération des demandes de colocations");
            }
        }
    }
}