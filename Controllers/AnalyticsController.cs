using ColocationAppBackend.BL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ColocationAppBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AnalyticsController : ControllerBase
    {
        private readonly AnalytiquesService _analyticsService;

        public AnalyticsController(AnalytiquesService analyticsService)
        {
            _analyticsService = analyticsService;
        }

        [Authorize(Roles = "Administrateur")]
        [HttpGet("VueDensemble")]
        public async Task<IActionResult> GetAnalytics()
        {
            var result = await _analyticsService.GetAnalyticsAsync();
            return Ok(result);
        }
    }
}
