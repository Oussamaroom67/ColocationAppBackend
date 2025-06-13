namespace ColocationAppBackend.Utils
{
    public static class LocationParser
    {
        public static (string Ville, string Quartier) ExtractVilleEtQuartier(string zone)
        {
            if (string.IsNullOrEmpty(zone)) return ("Inconnue", "Inconnu");

            var parts = zone.Split(',', StringSplitOptions.RemoveEmptyEntries);
            string quartier = parts.Length > 1 ? parts[0].Trim() : "Inconnu";
            string ville = parts.Length > 1 ? parts[1].Trim() : parts[0].Trim();

            return (ville, quartier);
        }
    }
}
