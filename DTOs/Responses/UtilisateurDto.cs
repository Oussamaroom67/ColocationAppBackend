namespace ColocationAppBackend.DTOs.Responses
{
    public class UtilisateurDto
    {
        public int Id { get; set; }
        public string Nom { get; set; }
        public string Prenom { get; set; }
        public string AvatarUrl { get; set; }
        public bool EstEnLigne { get; set; }
    }
}
