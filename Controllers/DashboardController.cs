using ColocationAppBackend.BL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
[Authorize(Roles = "Administrateur")]

[ApiController]
[Route("api/[controller]")]
public class DashboardController : ControllerBase
{
    private readonly DashboardService _dashboardService;

    public DashboardController(DashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }
    [Authorize(Roles = "Administrateur")]
    [HttpGet("overview")]
    public async Task<IActionResult> GetOverview()
    {
        var result = await _dashboardService.GetDashboardOverviewAsync();
        return Ok(result);
    }
}

