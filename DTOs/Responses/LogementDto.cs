namespace ColocationAppBackend.DTOs.Responses
{
    public class LogementDto
    {
        public int Id { get; set; }
        public string Titre { get; set; }
        public string Adresse { get; set; }
        public string ProprietaireNomComplet { get; set; }
        public string Type { get; set; }
        public string Prix { get; set; }
        public string Statut { get; set; }
        public DateTime DateAjout { get; set; }
    }
}
