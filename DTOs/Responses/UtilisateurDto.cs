namespace ColocationAppBackend.DTOs.Responses
{
    public class UtilisateurDto
    {
        public int Id { get; set; }
        public string Nom { get; set; }
        public string Email { get; set; }
        public string TypeUtilisateur { get; set; }
        public string Statut { get; set; }
        public string Verifie { get; set; }
        public int NbrUtilisateur { get; set; }
        public int NbrProprietaire { get; set; }
        public int NbrEtudiants { get; set; }
        public int NbrAdministrateurs { get; set; }
        public DateTime DateInscription { get; set; }
        public string Prenom { get; set; }
        public string AvatarUrl { get; set; }
        public bool EstEnLigne { get; set; }
    }
}
