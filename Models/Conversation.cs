namespace ColocationAppBackend.Models
{
    public class Conversation
    {
        public int Id { get; set; }

        public DateTime DateCreation { get; set; } = DateTime.Now;
        public int Utilisateur1Id { get; set; }
        public Utilisateur Utilisateur1 { get; set; }
        public int Utilisateur2Id { get; set; }
        public Utilisateur Utilisateur2 { get; set; }
        public List<Message> Messages { get; set; } = new();
        public string DernierMessage { get; set; }

        public DateTime? DateDernierMessage { get; set; }


    }
}
