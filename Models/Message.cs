namespace ColocationAppBackend.Models
{
    public class Message
    {
        public int Id { get; set; }
        public int ExpediteurId { get; set; }
        public Utilisateur Expediteur { get; set; }
        public int DestinataireId { get; set; }
        public Utilisateur Destinataire { get; set; }
        public string Contenu { get; set; }
        public DateTime DateEnvoi { get; set; } = DateTime.Now;
        public DateTime? DateLecture { get; set; } = null;
        public bool EstLu { get; set; } = false;
    }
}
