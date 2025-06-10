using ColocationAppBackend.BL;
using ColocationAppBackend.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ColocationAppBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class FavorisController : ControllerBase
    {
        private readonly FavoriService _favoriService;

        public FavorisController(FavoriService favoriService)
        {
            _favoriService = favoriService;
        }

        private int GetEtudiantId()
        {
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(idClaim, out var id) ? id : throw new UnauthorizedAccessException("Identifiant étudiant invalide.");
        }

        public class FavoriRequest
        {
            public int ElementId { get; set; }
            public TypeFavori Type { get; set; }
        }

        [HttpPost("ajouter")]
        public async Task<IActionResult> AjouterFavori([FromBody] FavoriRequest request)
        {
            await _favoriService.AjouterFavoriAsync(GetEtudiantId(), request.ElementId, request.Type);
            return Ok(new { message = "Ajouté aux favoris." });
        }

        [HttpPost("supprimer")]
        public async Task<IActionResult> SupprimerFavori([FromBody] FavoriRequest request)
        {
            await _favoriService.SupprimerFavoriAsync(GetEtudiantId(), request.ElementId, request.Type);
            return Ok(new { message = "Supprimé des favoris." });
        }

        [HttpGet("annoncesFavoris")]
        public async Task<IActionResult> GetFavorisAnnonces()
        {
            var annonces = await _favoriService.ObtenirFavorisAnnoncesAsync(GetEtudiantId());
            return Ok(annonces);
        }

        [HttpGet("colocationsFavoris")]
        public async Task<IActionResult> GetFavorisColocations()
        {
            var colocations = await _favoriService.ObtenirFavorisColocationsAsync(GetEtudiantId());
            return Ok(colocations);
        }
    }
}
