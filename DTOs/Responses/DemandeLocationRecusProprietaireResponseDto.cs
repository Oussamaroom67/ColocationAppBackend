using ColocationAppBackend.Enums;

namespace ColocationAppBackend.DTOs.Responses
{
    public class DemandeLocationRecusProprietaireResponseDto
    {
        public int Id { get; set; }
        public string Nom { get; set; }
        public string Email { get; set; }
        public string? Photo { get; set; }
        public string Logement { get; set; }
        public string DateDemande { get; set; }
        public string Budget { get; set; }
        public string Message { get; set; }
        public string? Universite { get; set; }
        public string? Annee { get; set; }
        public string Duree { get; set; }
        public string Emmenagement { get; set; }
        public string Status { get; set; }
        public string Priority { get; set; }

        // Informations supplémentaires
        public int AnnonceId { get; set; }
        public int EtudiantId { get; set; }
        public string? MessageReponse { get; set; }
        public DateTime? DateReponse { get; set; }
        public DateTime DateCreation { get; set; }
        public DateTime DateEmmenagementOriginal { get; set; }
        public int DureeSejour { get; set; }
        public int NbOccupants { get; set; }
        public LocationStatus StatusOriginal { get; set; }
    }
}
