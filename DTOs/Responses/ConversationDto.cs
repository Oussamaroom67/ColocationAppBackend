namespace ColocationAppBackend.DTOs.Responses
{
    public class ConversationDto
    {
        public int Id { get; set; }
        public DateTime DateCreation { get; set; }
        public DateTime? DateDernierMessage { get; set; }
        public string DernierMessage { get; set; }
        public UtilisateurDto AutreUtilisateur { get; set; }
        public int MessagesNonLus { get; set; }
    }
}
