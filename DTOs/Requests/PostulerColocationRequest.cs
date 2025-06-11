
using System.ComponentModel.DataAnnotations;

namespace ColocationAppBackend.DTOs.Requests
{
    public class PostulerColocationRequest
    {
        [Required(ErrorMessage = "L'ID de la colocation est requis")]
        public int ColocationId { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Le budget doit être positif")]
        public decimal? Budget { get; set; }

        [StringLength(500, ErrorMessage = "L'adresse ne peut pas dépasser 500 caractères")]
        public string? Adresse { get; set; }

        [Required(ErrorMessage = "Le message est requis")]
        [StringLength(1000, ErrorMessage = "Le message ne peut pas dépasser 1000 caractères")]
        public string Message { get; set; } = string.Empty;

        [Required(ErrorMessage = "La date d'emménagement est requise")]
        public DateTime DateEmmenagement { get; set; }

        public List<string>? Preferences { get; set; }
    }
}