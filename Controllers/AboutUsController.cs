using ColocationAppBackend.BL;
using Microsoft.AspNetCore.Mvc;
[ApiController]
[Route("api/[controller]")]
public class AboutUsController : ControllerBase
{
    private readonly AboutUsStatsService _aboutUsService;

    public AboutUsController(AboutUsStatsService aboutUsService)
    {
        _aboutUsService = aboutUsService;
    }

    [HttpGet("stats")]
    public async Task<IActionResult> GetStats()
    {
        var stats = await _aboutUsService.GetStats();
        return Ok(stats);
    }
}
