using ColocationAppBackend.Enums;
using System.ComponentModel.DataAnnotations;

namespace ColocationAppBackend.DTOs.Requests
{
    public class LogementAnnonceRequest
    {
        //ajouter l'ID de l'annonce pour les mises à jour
        public int? LogementId { get; set; }

        [Required(ErrorMessage = "L'identifiant du propriétaire est requis.")]
        public int ProprietaireId { get; set; }
        [Required(ErrorMessage = "Le titre est requis.")]
        [StringLength(100, ErrorMessage = "Le titre ne doit pas dépasser 100 caractères.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Le type de logement est requis.")]
        public string Type { get; set; }

        [Required(ErrorMessage = "La description est requise.")]
        [StringLength(1000, ErrorMessage = "La description ne doit pas dépasser 1000 caractères.")]
        public string Description { get; set; }

        [Range(100, 100000, ErrorMessage = "Le loyer mensuel doit être compris entre 100 et 100000.")]
        public decimal MonthlyRent { get; set; }

        [Range(0, 100000, ErrorMessage = "Le montant de la caution doit être positif.")]
        public decimal Deposit { get; set; }

        [Range(0, 10000, ErrorMessage = "Les charges doivent être positives.")]
        public decimal Fees { get; set; }

        [Required(ErrorMessage = "La date de disponibilité est requise.")]
        public string AvailableFrom { get; set; }

        [Required(ErrorMessage = "La durée souhaitée est requise.")]
        public string DesiredDuration { get; set; }

        [Required(ErrorMessage = "L'adresse est requise.")]
        public string Address { get; set; }

        [Required(ErrorMessage = "La ville est requise.")]
        public string City { get; set; }

        [Required(ErrorMessage = "Le code postal est requis.")]
        public string PostalCode { get; set; }

        [Required]
        public string Country { get; set; } = "Maroc";

        [Range(5, 1000, ErrorMessage = "La surface doit être comprise entre 5 et 1000 m².")]
        public decimal Surface { get; set; }

        [Range(1, 20, ErrorMessage = "Le nombre de pièces doit être compris entre 1 et 20.")]
        public int Rooms { get; set; }

        [Range(0, 10, ErrorMessage = "Le nombre de chambres doit être compris entre 0 et 10.")]
        public int Bedrooms { get; set; }

        [Range(0, 10, ErrorMessage = "Le nombre de salles de bain doit être compris entre 0 et 10.")]
        public int Bathrooms { get; set; }

        [Range(0, 100, ErrorMessage = "L'étage doit être compris entre 0 et 100.")]
        public int Floor { get; set; }

        public bool Furnished { get; set; }
        public bool PetsAllowed { get; set; }
        public bool SmokingAllowed { get; set; }

        public List<string> Amenities { get; set; } = new();

        [StringLength(500, ErrorMessage = "Les règles de la maison ne doivent pas dépasser 500 caractères.")]
        public string HouseRules { get; set; }

        public List<PhotoDto> Photos { get; set; } = new();
        
        public AnnonceStatus Status { get; set; }
    }
}