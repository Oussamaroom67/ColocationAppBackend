namespace ColocationAppBackend.DTOs.Responses
{
    public class MessageDto
    {
        public int Id { get; set; }
        public string Contenu { get; set; }
        public DateTime DateEnvoi { get; set; }
        public DateTime? DateLecture { get; set; }
        public bool EstLu { get; set; }
        public UtilisateurDto Expediteur { get; set; }
        public bool EstDeMoi { get; set; }
    }
}
