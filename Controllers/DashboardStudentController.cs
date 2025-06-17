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
    }
}