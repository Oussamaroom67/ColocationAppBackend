namespace ColocationAppBackend.Models
{
    public class Proprietaire : Utilisateur
    {
        protected Proprietaire() { 
            Logements = new HashSet<Logement>();
        }
        public string Adresse { get; set; }
        public string Ville { get; set; }
        public string CodePostal { get; set; }
        public string Pays { get; set; }
        public decimal NoteGlobale { get; set; }
        public int NombreProprietes { get; set; }
        public ICollection<Logement> Logements { get; set; }
    }
}
