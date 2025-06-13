using Microsoft.ML;
using ColocationAppBackend.DTOs.Responses;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ColocationAppBackend.ML
{
    public class ColocationRecommendationEngine
    {
        private readonly MLContext _mlContext;
        private ITransformer _model;
        private PredictionEngine<ColocationMatchingFeatures, ScorePrediction> _predictionEngine;
        private const float RECOMMENDATION_THRESHOLD = 0.6f;

        public ColocationRecommendationEngine()
        {
            _mlContext = new MLContext(seed: 0);
            InitializeModel();
        }

        private void InitializeModel()
        {
            try
            {
                var trainingData = GenerateSyntheticTrainingData();
                var dataView = _mlContext.Data.LoadFromEnumerable(trainingData);

                var pipeline = _mlContext.Transforms.Concatenate("Features",
                        nameof(ColocationMatchingFeatures.BudgetSimilarity),
                        nameof(ColocationMatchingFeatures.SchoolMatch),
                        nameof(ColocationMatchingFeatures.ZoneMatch),
                        nameof(ColocationMatchingFeatures.DateProximity),
                        nameof(ColocationMatchingFeatures.PreferenceCompatibility))
                    .Append(_mlContext.Regression.Trainers.Sdca(
                        labelColumnName: nameof(ColocationMatchingFeatures.Score),
                        maximumNumberOfIterations: 100));

                _model = pipeline.Fit(dataView);

                // Créer l'engine de prédiction une seule fois
                _predictionEngine = _mlContext.Model.CreatePredictionEngine<ColocationMatchingFeatures, ScorePrediction>(_model);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur initialisation ML: {ex.Message}");
                // En cas d'erreur, utiliser un calcul simple
                _model = null;
            }
        }

        public float PredictScore(ColocationMatchingFeatures features)
        {
            try
            {
                if (_predictionEngine == null || _model == null)
                {
                    // Fallback : calcul simple de score
                    return CalculateSimpleScore(features);
                }

                var prediction = _predictionEngine.Predict(features);
                return Math.Max(0, Math.Min(1, prediction.Score));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur prédiction ML: {ex.Message}");
                return CalculateSimpleScore(features);
            }
        }

        private float CalculateSimpleScore(ColocationMatchingFeatures features)
        {
            // Calcul de score simple sans ML en cas de problème
            var weights = new
            {
                Budget = 0.3f,
                School = 0.2f,
                Zone = 0.25f,
                Date = 0.15f,
                Preferences = 0.1f
            };

            return (features.BudgetSimilarity * weights.Budget +
                   features.SchoolMatch * weights.School +
                   features.ZoneMatch * weights.Zone +
                   features.DateProximity * weights.Date +
                   features.PreferenceCompatibility * weights.Preferences);
        }

        public bool IsRecommended(float score)
        {
            return score >= RECOMMENDATION_THRESHOLD;
        }

        private List<ColocationMatchingFeatures> GenerateSyntheticTrainingData()
        {
            return new List<ColocationMatchingFeatures>
            {
                // Correspondances parfaites
                new() { BudgetSimilarity = 1.0f, SchoolMatch = 1.0f, ZoneMatch = 1.0f, DateProximity = 1.0f, PreferenceCompatibility = 1.0f, Score = 0.95f },
                new() { BudgetSimilarity = 0.9f, SchoolMatch = 1.0f, ZoneMatch = 1.0f, DateProximity = 0.8f, PreferenceCompatibility = 0.9f, Score = 0.9f },
                
                // Bonnes correspondances
                new() { BudgetSimilarity = 0.8f, SchoolMatch = 0.0f, ZoneMatch = 1.0f, DateProximity = 0.9f, PreferenceCompatibility = 0.7f, Score = 0.75f },
                new() { BudgetSimilarity = 0.7f, SchoolMatch = 1.0f, ZoneMatch = 0.0f, DateProximity = 0.8f, PreferenceCompatibility = 0.8f, Score = 0.7f },
                
                // Correspondances moyennes
                new() { BudgetSimilarity = 0.6f, SchoolMatch = 0.0f, ZoneMatch = 1.0f, DateProximity = 0.5f, PreferenceCompatibility = 0.5f, Score = 0.55f },
                new() { BudgetSimilarity = 0.5f, SchoolMatch = 0.0f, ZoneMatch = 0.0f, DateProximity = 0.7f, PreferenceCompatibility = 0.6f, Score = 0.45f },
                
                // Mauvaises correspondances
                new() { BudgetSimilarity = 0.3f, SchoolMatch = 0.0f, ZoneMatch = 0.0f, DateProximity = 0.2f, PreferenceCompatibility = 0.1f, Score = 0.2f },
                new() { BudgetSimilarity = 0.1f, SchoolMatch = 0.0f, ZoneMatch = 0.0f, DateProximity = 0.1f, PreferenceCompatibility = 0.0f, Score = 0.1f },
                
                // Données supplémentaires
                new() { BudgetSimilarity = 0.75f, SchoolMatch = 1.0f, ZoneMatch = 0.5f, DateProximity = 0.6f, PreferenceCompatibility = 0.8f, Score = 0.8f },
                new() { BudgetSimilarity = 0.4f, SchoolMatch = 0.5f, ZoneMatch = 0.8f, DateProximity = 0.3f, PreferenceCompatibility = 0.4f, Score = 0.35f },
                new() { BudgetSimilarity = 0.85f, SchoolMatch = 0.0f, ZoneMatch = 0.5f, DateProximity = 0.9f, PreferenceCompatibility = 0.6f, Score = 0.68f },
                new() { BudgetSimilarity = 0.2f, SchoolMatch = 1.0f, ZoneMatch = 0.3f, DateProximity = 0.4f, PreferenceCompatibility = 0.2f, Score = 0.4f },
            };
        }

        private class ScorePrediction
        {
            public float Score { get; set; }
        }

        // Libérer les ressources
        public void Dispose()
        {
            _predictionEngine?.Dispose();
        }
    }
}