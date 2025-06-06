namespace ColocationAppBackend.Models
{
    public class ReseauSocial
    {
        public string Nom { get; set; } 
        public string Lien { get; set; }
        public int EtudiantId { get; set; }
        public Etudiant Etudiant { get; set; }
    }
}
