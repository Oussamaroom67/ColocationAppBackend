using ColocationAppBackend.Enums;

namespace ColocationAppBackend.DTOs.Requests
{
    public class RegisterEtudiantDto
    {
        public string Nom { get; set; }
        public string Prenom { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Telephone { get; set; }
        public string PhotoUrl { get; set; }
        public string Universite { get; set; }
        public string DomaineEtudes { get; set; }
        public string NiveauEtudes { get; set; }
        public string Adresse { get; set; }
        public decimal Budget { get; set; }
        public string Bio { get; set; }
        public List<Habitude> Habitudes { get; set; } = new();
        public List<CentreInteret> CentresInteret { get; set; } = new();
        public List<StyleDeVie> StyleDeVie { get; set; } = new();
    }
}
