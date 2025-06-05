namespace ColocationAppBackend.Models
{
    public class Proprietaire : Utilisateur
    {
        public string Adresse { get; set; }
        public string Ville { get; set; }
        public string CodePostal { get; set; }
        public string Pays { get; set; }
        public decimal NoteGlobale { get; set; }
        public int NombreProprietes { get; set; }
    }
}
