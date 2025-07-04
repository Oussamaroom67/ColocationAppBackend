using ColocationAppBackend.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ColocationAppBackend.Models
{
    public class Logement
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "L'adresse est obligatoire.")]
        public string Adresse { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        [Range(5, 1000, ErrorMessage = "La surface doit être comprise entre 5 et 1000 m².")]
        public decimal Surface { get; set; }

        [Range(0, 100, ErrorMessage = "Le nombre de chambres doit être compris entre 0 et 100.")]
        public int NbChambres { get; set; }

        [Required(ErrorMessage = "La ville est obligatoire.")]
        public string Ville { get; set; }

        [Required(ErrorMessage = "Le code postal est obligatoire.")]
        public string CodePostal { get; set; }

        [Required(ErrorMessage = "Le pays est obligatoire.")]
        public string Pays { get; set; }

        [Column(TypeName = "decimal(9,6)")]
        public decimal Latitude { get; set; }

        [Column(TypeName = "decimal(9,6)")]
        public decimal Longitude { get; set; }

        public int NbSallesBain { get; set; }

        public int? Etage { get; set; }

        public int? NbEtagesTotal { get; set; }

        public bool EstMeuble { get; set; }

        public bool AnimauxAutorises { get; set; }

        public bool FumeurAutorise { get; set; }

        public bool InternetInclus { get; set; }

        public bool ChargesIncluses { get; set; }

        public bool ParkingDisponible { get; set; }
        public LogementStatus status { get; set; } =  LogementStatus.EnAttente;
        //type de logement
        public string Type { get; set; }

        [ForeignKey("Proprietaire")]
        [Required(ErrorMessage = "L'identifiant du propriétaire est obligatoire.")]
        public int ProprietaireId { get; set; }

        public Proprietaire Proprietaire { get; set; }

        public Annonce Annonce { get; set; }
    }
}
