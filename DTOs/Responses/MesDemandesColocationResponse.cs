namespace ColocationAppBackend.DTOs.Responses
{
    public class MesDemandesColocationResponse
    {
        public int Id { get; set; }
        public string Nom { get; set; }
        public string Ecole { get; set; }
        public string Message { get; set; }
        public string Date { get; set; }
        public string Statut { get; set; }
        public string? Budget { get; set; }
        public string? Quartier { get; set; }
        public List<string>? Preferences { get; set; }
        public string? Reponse { get; set; } // Réponse du propriétaire
        
        // Informations sur la colocation ciblée
        public int ColocationId { get; set; }
        public string ColocationAdresse { get; set; }
        public string ColocationBudget { get; set; }
        public List<string> ColocationPreferences { get; set; }
    }
}
