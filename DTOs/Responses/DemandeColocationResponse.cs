
namespace ColocationAppBackend.DTOs.Responses
{
    public class DemandeColocationResponse
    {
        public int DemandeId { get; set; }
        public decimal? Budget { get; set; }
        public string? Adresse { get; set; }
        public string Message { get; set; } = string.Empty;
        public DateTime DateEmmenagement { get; set; }
        public List<string> Preferences { get; set; } = new List<string>();
        public string Statut { get; set; } = string.Empty;
        public string NomEtudiant { get; set; } = string.Empty;
        public DateTime DateCreation { get; set; }

        // Informations sur la colocation
        public int ColocationId { get; set; }
        public string ColocationAdresse { get; set; } = string.Empty;
        public string ProprietaireColocation { get; set; } = string.Empty;
        public double ColocationBudget { get; set; }
    }
}