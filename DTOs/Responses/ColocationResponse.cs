namespace ColocationAppBackend.DTOs.Responses
{
    public class ColocationResponse
    {
        public int ColocationId { get; set; }
        public string Adresse { get; set; }
        public double Budget { get; set; }
        public string Type { get; set; }
        public DateTime DateDebutDisponibilite { get; set; }
        public List<string> Preferences { get; set; }
        public string NomEtudiant { get; set; }
    }
}
