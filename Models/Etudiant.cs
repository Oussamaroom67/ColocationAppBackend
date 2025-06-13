namespace ColocationAppBackend.Models
{
    public class Etudiant : Utilisateur
    {
        public Etudiant()
        {
            Colocations = new HashSet<Colocation>();
            Favoris = new HashSet<Favori>();
            DemandesColocations = new HashSet<DemandeColocation>();
            ReseauxSociaux = new HashSet<ReseauSocial>();
        }
        public string NiveauEtudes { get; set; }
        public string Adresse { get; set; }
        public string Universite { get; set; }
        public String DomaineEtudes { get; set; }
        public decimal Budget { get; set; }
        public string Bio {  get; set; }
        public List<string> Habitudes { get; set; } = [];
        public List<string> CentresInteret { get; set; } = [];
        public List<string> StyleDeVie { get; set; } = [];
        public ICollection<Colocation> Colocations { get; set; }
        public ICollection<Favori> Favoris { get; set; }
        public ICollection<DemandeColocation> DemandesColocations {  get; set; }
        public ICollection<ReseauSocial> ReseauxSociaux { get; set; }
        public ICollection<DemandeLocation> DemandesLocation { get; set; }


    }
}
