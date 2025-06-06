using ColocationAppBackend.Enums;

namespace ColocationAppBackend.Models
{
    public class Colocation
    {
        public int Id { get; set; }
        public string Adresse { get; set; }
        public double Budget { get; set; }
        public ColocationType Type { get; set; }
        public DateTime DateDebutDisponibilite { get; set; }
        public List<string> Preferences { get; set; } 
        public int EtudiantId { get; set; }
        public Etudiant Etudiant { get; set; }

    }
}
