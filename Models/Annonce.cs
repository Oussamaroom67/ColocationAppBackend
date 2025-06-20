using ColocationAppBackend.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ColocationAppBackend.Models
{
    public class Annonce
    {
        public Annonce()
        {
            Photos = new HashSet<Photo>();
            Signalements = new HashSet<Signalement>();
        }

        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Le titre est obligatoire.")]
        [MaxLength(200, ErrorMessage = "Le titre ne peut pas dépasser 200 caractères.")]
        public string Titre { get; set; }

        [Required(ErrorMessage = "La description est obligatoire.")]
        public string Description { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        [Range(0, 100000, ErrorMessage = "Le prix doit être un montant positif.")]
        public decimal Prix { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        [Range(0, 50000, ErrorMessage = "La caution doit être un montant positif.")]
        public decimal Caution { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        [Range(0, 10000, ErrorMessage = "Les charges doivent être un montant positif.")]
        public decimal Charges { get; set; }

        [Required(ErrorMessage = "La date de disponibilité est obligatoire.")]
        public DateTime DisponibleDe { get; set; }

        [Required(ErrorMessage = "La date de fin de disponibilité est obligatoire.")]
        public DateTime DisponibleJusqu { get; set; }

        [Range(1, 60, ErrorMessage = "La durée minimale doit être comprise entre 1 et 60 mois.")]
        public int DureeMinMois { get; set; }

        [Range(1, 50, ErrorMessage = "Le nombre maximum d’occupants doit être compris entre 1 et 50.")]
        public int OccupantsMax { get; set; }

        [Required(ErrorMessage = "Le statut de l'annonce est obligatoire.")]
        public AnnonceStatus Statut { get; set; }

        public int NbVues { get; set; } = 0;

        public DateTime DateModification { get; set; } = DateTime.Now;
        public VerificationLogementStatut StatutVerification = VerificationLogementStatut.EnAttente;

        // Relation avec Logement
        [ForeignKey("Logement")]
        [Required(ErrorMessage = "Le logement associé est obligatoire.")]
        public int LogementId { get; set; }

        public virtual Logement Logement { get; set; }

        // Relation avec les Photos
        public virtual ICollection<Photo> Photos { get; set; }

        // Relation avec les Signalements
        public virtual ICollection<Signalement> Signalements { get; set; }
        public ICollection<DemandeLocation> DemandesLocation { get; set; }
    }
}
