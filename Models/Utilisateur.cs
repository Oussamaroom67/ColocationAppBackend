namespace ColocationAppBackend.Models
{
    public abstract class Utilisateur
    {
        public int Id { get; set; }
        public string Nom { get; set; }
        public string Email { get; set; }
        public string MotDePasse { get; set; }
        public string? Telephone { get; set; }
        public string Prenom { get; set; }
        public DateTime DateInscription { get; set; }
        public bool EstActif { get; set; }
        public bool EstVerifie { get; set; }
        public DateTime DernierConnexion { get; set; }
        public DateTime DateModification { get; set; }
        public string? AvatarUrl { get; set; }

    }
}
