namespace ColocationAppBackend.DTOs.Responses
{
    public class DemandesRecuesResponse
    {
        public int DemandeId { get; set; }
        public string NomEtudiant { get; set; }
        public string EcoleEtudiant { get; set; }
        public string Message { get; set; }
        public string? Budget { get; set; }
        public string DateEmmenagement { get; set; }
        public List<string> Preferences { get; set; }
        public string Statut { get; set; }
        public string ColocationAdresse { get; set; }
        public string? ReponseEnvoyee { get; set; }
        public string? DateReponse { get; set; }
    }
}
