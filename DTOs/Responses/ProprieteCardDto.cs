namespace ColocationAppBackend.DTOs.Responses
{
    public class ProprieteCardDto
    {
        public int Id { get; set; }
        public string Titre { get; set; }
        public decimal Prix { get; set; }
        public string Statut { get; set; }
        public int NbVues { get; set; }
        public int NbDemandes { get; set; }
        public string? PhotoUrl { get; set; }
        public int LogementId { get; set; }
        public DateTime DateModification { get; set; }
    }
}
