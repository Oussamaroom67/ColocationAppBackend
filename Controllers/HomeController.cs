using ColocationAppBackend.BL;
using ColocationAppBackend.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ColocationAppBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly HomeService _homeService;
        public HomeController(HomeService homeService)
        {
            _homeService = homeService;
        }
        [HttpGet]
        [Route("statistic")]
        public async Task<IActionResult> GetStatistic()
        {
            var data =await _homeService.GetStatistic();
            return Ok(data);
        }
        [HttpGet]
        [Route("Featured")]
        public async Task<IActionResult> GetFeatured()
        {
            var data = await _homeService.GetFeatured();
            return Ok(data);
        }
    }
}
