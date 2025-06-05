namespace ColocationAppBackend.Models
{
    public class Etudiant : Utilisateur
    {
        public string NiveauEtudes { get; set; }
        public string Adresse { get; set; }
        public string Universite { get; set; }
        public String DomaineEtudes { get; set; }
        public decimal Budget { get; set; }
        public string Bio {  get; set; }
        public List<string> Habitudes { get; set; } = [];

        public List<string> CentresInteret { get; set; } = [];
        public List<string> StyleDeVie { get; set; } = [];
        public List<ReseauSocial> ReseauxSociaux { get; set; }= [];

    }
}
