namespace ColocationAppBackend.DTOs.Responses
{
    public class ColocationMatchingFeatures
    {
        public float BudgetSimilarity { get; set; }
        public float SchoolMatch { get; set; }
        public float ZoneMatch { get; set; }
        public float DateProximity { get; set; }
        public float PreferenceCompatibility { get; set; }
        public float Score { get; set; } // Label pour l'entraînement
    }
}
