namespace ColocationAppBackend.Models
{
    public class Photo
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public DateTime DateAjout { get; set; }
        public int AnnonceId { get; set; }
        public Annonce Annonce { get; set; }
    }
}
