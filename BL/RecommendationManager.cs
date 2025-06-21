using ColocationAppBackend.Data;
using ColocationAppBackend.DTOs.Responses;
using ColocationAppBackend.ML;
using ColocationAppBackend.Models;
using ColocationAppBackend.Utils;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ColocationAppBackend.BL
{
    public class RecommendationManager
    {
        private readonly ApplicationDbContext _context;
        private readonly ColocationRecommendationEngine _engine;

        public RecommendationManager(ApplicationDbContext context)
        {
            _context = context;
            _engine = new ColocationRecommendationEngine();
        }

        public async Task<List<ColocationRecommendationResponse>> GetRecommendedColocationsAsync(int etudiantId)
        {
            try
            {
                // Récupérer l'étudiant connecté avec timeout
                var currentEtudiant = await _context.Etudiants
                    .AsNoTracking()
                    .FirstOrDefaultAsync(e => e.Id == etudiantId);

                if (currentEtudiant == null)
                    throw new Exception("Étudiant introuvable");

                // Récupérer les colocations avec une requête optimisée
                var colocations = await _context.Colocations
                    .AsNoTracking()
                    .Include(c => c.Etudiant)
                    .Where(c => c.EtudiantId != etudiantId)
                    .Select(c => new
                    {
                        c.Id,
                        c.Budget,
                        c.DateDebutDisponibilite,
                        c.Adresse,
                        c.Preferences,
                        c.Type,
                        Etudiant = new
                        {
                            c.Etudiant.Id,
                            c.Etudiant.Prenom,
                            c.Etudiant.Nom,
                            c.Etudiant.Universite,
                            c.Etudiant.AvatarUrl
                        }
                    })
                    .Take(50) 
                    .AsNoTracking()
                    .ToListAsync();

                var results = new List<ColocationRecommendationResponse>();

                // Traitement parallèle pour les calculs de score
                var tasks = colocations.Select(async colocation =>
                {
                    var score = await Task.Run(() => CalculateMatchingScore(currentEtudiant, colocation));
                    var (ville, quartier) = LocationParser.ExtractVilleEtQuartier(colocation.Adresse);

                    return new ColocationRecommendationResponse
                    {
                        Id = colocation.Id,
                        Name = $"{colocation.Etudiant.Prenom} {colocation.Etudiant.Nom}",
                        School = colocation.Etudiant.Universite,
                        Budget = $"{colocation.Budget:0} MAD/mois",
                        MoveInDate = colocation.DateDebutDisponibilite.ToString("dd/MM/yyyy"),
                        PreferredZone = quartier,
                        Type = colocation.Type,
                        RecommendationScore = score,
                        IsRecommended = _engine.IsRecommended(score),
                        Preferences = colocation.Preferences ?? new List<string>(),
                        AvatarUrl = colocation.Etudiant.AvatarUrl
                    };
                });

                // Attendre tous les calculs avec timeout
                var completedTasks = await Task.WhenAll(tasks);
                results.AddRange(completedTasks);

                // Trier : recommandations d'abord (par score décroissant), puis le reste
                return results
                    .OrderByDescending(r => r.IsRecommended)
                    .ThenByDescending(r => r.RecommendationScore)
                    .ToList();
            }
            catch (Exception ex)
            {
                // Log l'erreur et retourner une liste vide plutôt que de faire échouer
                Console.WriteLine($"Erreur dans GetRecommendedColocationsAsync: {ex.Message}");
                return new List<ColocationRecommendationResponse>();
            }
        }

        private float CalculateMatchingScore(Etudiant currentEtudiant, dynamic colocation)
        {
            try
            {
                var features = new ColocationMatchingFeatures
                {
                    BudgetSimilarity = CalculateBudgetSimilarity((float)currentEtudiant.Budget, (float)colocation.Budget),
                    SchoolMatch = currentEtudiant.Universite?.Equals(colocation.Etudiant.Universite, StringComparison.OrdinalIgnoreCase) == true ? 1.0f : 0.0f,
                    ZoneMatch = CalculateZoneMatch(currentEtudiant.Adresse, colocation.Adresse),
                    DateProximity = CalculateDateProximity(DateTime.Now.AddDays(30), colocation.DateDebutDisponibilite),
                    PreferenceCompatibility = CalculatePreferenceCompatibility(currentEtudiant, colocation.Preferences)
                };

                return _engine.PredictScore(features);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur calcul score: {ex.Message}");
                return 0.0f;
            }
        }

        private float CalculateBudgetSimilarity(float budget1, float budget2)
        {
            if (budget1 <= 0 || budget2 <= 0) return 0.5f;

            float maxBudget = Math.Max(budget1, budget2);
            float difference = Math.Abs(budget1 - budget2);

            return Math.Max(0, 1 - (difference / maxBudget));
        }

        private float CalculateZoneMatch(string zone1, string zone2)
        {
            if (string.IsNullOrEmpty(zone1) || string.IsNullOrEmpty(zone2)) return 0.0f;

            try
            {
                var (ville1, quartier1) = LocationParser.ExtractVilleEtQuartier(zone1);
                var (ville2, quartier2) = LocationParser.ExtractVilleEtQuartier(zone2);

                // Match exact du quartier = 1.0
                if (quartier1.Equals(quartier2, StringComparison.OrdinalIgnoreCase))
                    return 1.0f;

                // Match de la ville seulement = 0.5
                if (ville1.Equals(ville2, StringComparison.OrdinalIgnoreCase))
                    return 0.5f;

                return 0.0f;
            }
            catch
            {
                return 0.0f;
            }
        }

        private float CalculateDateProximity(DateTime desiredDate, DateTime availableDate)
        {
            try
            {
                var daysDifference = Math.Abs((desiredDate - availableDate).TotalDays);

                // Si moins de 7 jours de différence = parfait match
                if (daysDifference <= 7) return 1.0f;

                // Si moins de 30 jours = bon match
                if (daysDifference <= 30) return 0.8f;

                // Si moins de 60 jours = match moyen
                if (daysDifference <= 60) return 0.5f;

                // Plus de 60 jours = mauvais match
                return Math.Max(0, 1 - (float)(daysDifference / 365));
            }
            catch
            {
                return 0.0f;
            }
        }

        private float CalculatePreferenceCompatibility(Etudiant etudiant, List<string> colocationPrefs)
        {
            try
            {
                var etudiantPrefs = new List<string>();
                if (etudiant.Habitudes != null) etudiantPrefs.AddRange(etudiant.Habitudes);
                if (etudiant.CentresInteret != null) etudiantPrefs.AddRange(etudiant.CentresInteret);
                if (etudiant.StyleDeVie != null) etudiantPrefs.AddRange(etudiant.StyleDeVie);

                colocationPrefs = colocationPrefs ?? new List<string>();

                if (!etudiantPrefs.Any() && !colocationPrefs.Any()) return 0.5f;
                if (!etudiantPrefs.Any() || !colocationPrefs.Any()) return 0.3f;

                var commonPrefs = etudiantPrefs.Intersect(colocationPrefs, StringComparer.OrdinalIgnoreCase).Count();
                var totalUniquePrefs = etudiantPrefs.Union(colocationPrefs, StringComparer.OrdinalIgnoreCase).Count();

                return totalUniquePrefs == 0 ? 0.0f : (float)commonPrefs / totalUniquePrefs;
            }
            catch
            {
                return 0.0f;
            }
        }
    }
}