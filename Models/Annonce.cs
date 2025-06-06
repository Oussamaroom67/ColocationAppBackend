using ColocationAppBackend.Enums;

namespace ColocationAppBackend.Models
{
    public class Annonce
    {
        protected Annonce()
        {
            Photos = new HashSet<Photo>();
            Signalements = new HashSet<Signalement>();
        }
        public int Id { get; set; }
        public string Titre { get; set; }
        public string Description { get; set;}
        public decimal Prix { get; set; }
        public decimal Caution { get; set; }
        public decimal Charges { get; set; }
        public DateTime DisponibleDe { get; set; }
        public DateTime DisponibleJusqu { get; set; }
        public int DureeMinMois { get; set; }
        public int OccupantsMax { get; set; }
        public AnnonceStatus Statut { get; set; }
        public int NbVues { get; set; }
        public DateTime DateModification { get; set; }
        public int LogementId { get; set; } 
        public Logement Logement { get; set; }
        public ICollection<Photo> Photos { get; set; }
        public ICollection<Signalement>? Signalements { get; set; }

    }
}
