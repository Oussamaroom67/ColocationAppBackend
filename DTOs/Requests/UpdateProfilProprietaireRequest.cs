namespace ColocationAppBackend.DTOs.Requests
{
    public class UpdateProfilProprietaireRequest
    {
        public string Nom { get; set; }
        public string Prenom { get; set; }
        public string Email { get; set; }
        public string? Telephone { get; set; }
        public string? Adresse { get; set; }
        public string? Ville { get; set; }
        public string? CodePostal { get; set; }
        public string? Pays { get; set; }
        public string? AvatarUrl { get;set; }

    }
}
