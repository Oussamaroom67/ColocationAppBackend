using ColocationAppBackend.BL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ColocationAppBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Etudiant")]
    public class RecommendationController : ControllerBase
    {
        private readonly RecommendationManager _manager;

        public RecommendationController(RecommendationManager manager)
        {
            _manager = manager;
        }

        [HttpGet("colocations")]
        public async Task<IActionResult> GetRecommendedColocations()
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int etudiantId))
                {
                    return Unauthorized(new { error = "Utilisateur non identifié" });
                }

                // Ajouter un timeout pour éviter les requêtes infinies
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));

                var result = await _manager.GetRecommendedColocationsAsync(etudiantId);

                return Ok(new
                {
                    success = true,
                    data = result,
                    totalCount = result.Count,
                    recommendedCount = result.Count(r => r.IsRecommended),
                    message = result.Count == 0 ? "Aucune colocation trouvée pour le moment" : null
                });
            }
            catch (OperationCanceledException)
            {
                return StatusCode(408, new { error = "La requête a pris trop de temps à s'exécuter" });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    error = "Erreur lors de la récupération des recommandations",
                    details = ex.Message
                });
            }
        }

        [HttpGet("colocations/stats")]
        public async Task<IActionResult> GetRecommendationStats()
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int etudiantId))
                {
                    return Unauthorized(new { error = "Utilisateur non identifié" });
                }

                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(15));

                var result = await _manager.GetRecommendedColocationsAsync(etudiantId);

                var stats = new
                {
                    TotalColocations = result.Count,
                    RecommendedColocations = result.Count(r => r.IsRecommended),
                    AverageScore = result.Any() ? Math.Round(result.Average(r => r.RecommendationScore), 2) : 0,
                    HighScoreColocations = result.Count(r => r.RecommendationScore >= 0.8f),
                    MediumScoreColocations = result.Count(r => r.RecommendationScore >= 0.5f && r.RecommendationScore < 0.8f),
                    LowScoreColocations = result.Count(r => r.RecommendationScore < 0.5f)
                };

                return Ok(new { success = true, stats });
            }
            catch (OperationCanceledException)
            {
                return StatusCode(408, new { error = "La requête a pris trop de temps à s'exécuter" });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    error = "Erreur lors de la récupération des statistiques",
                    details = ex.Message
                });
            }
        }

        // Endpoint de test simple pour vérifier que l'API fonctionne
        [HttpGet("test")]
        public IActionResult Test()
        {
            return Ok(new
            {
                message = "API de recommandation fonctionnelle",
                timestamp = DateTime.Now,
                userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            });
        }
    }
}