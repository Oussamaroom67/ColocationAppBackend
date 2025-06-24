using ColocationAppBackend.BL;
using Microsoft.AspNetCore.Mvc;

namespace ColocationAppBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AnnonceController : ControllerBase
    {
        private readonly AnnonceService _service;

        public AnnonceController(AnnonceService annonceService)
        {
            _service = annonceService;
        }

        [HttpGet("{id}")]
        public IActionResult GetAnnonceById(int id)
        {
            var annonce = _service.GetById(id);
            if (annonce == null)
                return NotFound("Annonce non trouvée.");

            return Ok(annonce);
        }
        [HttpGet("{id}/similar")]
        public IActionResult GetSimilarAnnonces(int id)
        {
            var annonces = _service.GetSimilarAnnonces(id);
            if (annonces == null)
                return NotFound("Annonce non trouvée .");
            return Ok(annonces);
        }

    }
}
