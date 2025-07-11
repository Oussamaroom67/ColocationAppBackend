namespace ColocationAppBackend.DTOs.Responses
{
    public class ProfilProprietaireDto
    {
        public int Id { get; set; }
        public string Nom { get; set; }
        public string Prenom { get; set; }
        public string Email { get; set; }
        public string? Telephone { get; set; }
        public string? Adresse { get; set; }
        public string? Ville { get; set; }
        public string? CodePostal { get; set; }
        public string? Pays { get; set; }
        public string? AvatarUrl { get; set; }
        public decimal NoteGlobale { get; set; }
        public int NombreProprietes { get; set; }
        public bool EstVerifie { get; set; }
        public DateTime DateInscription { get; set; }
        public string Status { get; set; }
    }
}
