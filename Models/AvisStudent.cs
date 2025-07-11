namespace ColocationAppBackend.Models
{
    public class AvisStudent
    {
        public int Id { get; set; }
        public double rating { get; set; }
        public string comment { get; set; }
        public int StudentId { get; set; }
        public Utilisateur Student { get; set; }
        public int ProprietaireId { get; set; }
        public Utilisateur Proprietaire { get; set; }
    }
}
