using ColocationAppBackend.Enums;

namespace ColocationAppBackend.Models
{
    public class Favori
    {
        public int Id { get; set; }
        public int EtudiantId { get; set; }
        public Etudiant Etudiant { get; set; }

        // Favori lié à une annonce
        public int? AnnonceId { get; set; }
        public Annonce? Annonce { get; set; }

        // Favori lié à une offre de colocation
        public int? OffreColocationId { get; set; }
        public Colocation? OffreColocation { get; set; }

        // "Annonce" ou "Colocation"
        public TypeFavori Type{ get; set; }

        public DateTime DateAjout { get; set; } = DateTime.UtcNow;

    }
}
