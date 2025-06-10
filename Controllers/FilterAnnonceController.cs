using ColocationAppBackend.BL;
using ColocationAppBackend.DTOs.Requests;
using ColocationAppBackend.Models;
using Microsoft.AspNetCore.Mvc;

namespace ColocationAppBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FilterAnnonceController : ControllerBase
    {
        private readonly AnnonceFilterService _filterService;

        public FilterAnnonceController(AnnonceFilterService filterService)
        {
            _filterService = filterService;
        }

        [HttpPost("basic")]
        public ActionResult<List<Annonce>> FilterBasic([FromBody] BasicFilterDTO filter)
        {
            try
            {
                var result = _filterService.FilterBasic(filter);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erreur lors du filtrage: {ex.Message}");
            }
        }

        [HttpPost("advanced")]
        public ActionResult<List<Annonce>> FilterAdvanced([FromBody] AdvancedFilterDTO filter)
        {
            try
            {
                var result = _filterService.FilterAdvanced(filter);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erreur lors du filtrage: {ex.Message}");
            }
        }
    }
}
