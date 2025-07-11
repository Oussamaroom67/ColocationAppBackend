using ColocationAppBackend.BL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ColocationAppBackend.Controllers
{

        [ApiController]
        [Route("api/[controller]")]
        [Authorize] 
        public class DashboardOwnerController : ControllerBase
        {
            private readonly DashboardOwnerService _dashboardService;

            public DashboardOwnerController(DashboardOwnerService dashboardService)
            {
                _dashboardService = dashboardService;
            }

            [HttpGet("stats")]
            public async Task<IActionResult> GetDashboardStats()
            {
                try
                {
                    var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int proprietaireId))
                    {
                        return Unauthorized(new { error = "Utilisateur non identifié" });
                    }

                    var stats = await _dashboardService.GetDashboardStatsAsync(proprietaireId);
                    return Ok(stats);
                }
                catch (Exception ex)
                {
                    return BadRequest(new { message = "Erreur lors de la récupération des statistiques", error = ex.Message });
                }
            }

            [HttpGet("proprietes")]
            public async Task<IActionResult> GetProprietes()
            {
                try
                {
                    var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int proprietaireId))
                    {
                        return Unauthorized(new { error = "Utilisateur non identifié" });
                    }

                    var proprietes = await _dashboardService.GetProprietesAsync(proprietaireId);
                    return Ok(proprietes);
                }
                catch (Exception ex)
                {
                    return BadRequest(new { message = "Erreur lors de la récupération des propriétés", error = ex.Message });
                }
            }


        [HttpGet("recent-inquiries")]
        public async Task<IActionResult> GetRecentInquiries([FromQuery] int limit = 3)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int proprietaireId))
                {
                    return Unauthorized(new { error = "Utilisateur non identifié" });
                }

                var inquiries = await _dashboardService.GetRecentInquiriesAsync(proprietaireId, limit);
                return Ok(inquiries);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Erreur lors de la récupération des demandes récentes", error = ex.Message });
            }
        }
        [HttpGet("analytics")]
        public async Task<IActionResult> GetPropertyAnalytics()
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int proprietaireId))
                {
                    return Unauthorized(new { error = "Utilisateur non identifié" });
                }

                var analytics = await _dashboardService.GetPropertyAnalyticsAsync(proprietaireId);
                return Ok(analytics);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Erreur lors de la récupération des analyses", error = ex.Message });
            }
        }

        [HttpGet("recent-activities")]
        public async Task<IActionResult> GetRecentActivities([FromQuery] int limit = 10)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int proprietaireId))
                {
                    return Unauthorized(new { error = "Utilisateur non identifié" });
                }

                var activities = await _dashboardService.GetRecentActivitiesAsync(proprietaireId, limit);
                return Ok(activities);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Erreur lors de la récupération des activités récentes", error = ex.Message });
            }
        }

    }

}
