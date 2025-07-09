using ColocationAppBackend.Enums;

namespace ColocationAppBackend.DTOs.Responses
{
    public class DemandeLocationResponseDto
    {
        public int Id { get; set; }
        public int AnnonceId { get; set; }
        public string TitreAnnonce { get; set; }
        public int EtudiantId { get; set; }
        public string NomEtudiant { get; set; }
        public string Message { get; set; }
        public DateTime DateEmmenagement { get; set; }
        public int DureeSejour { get; set; }
        public int NbOccupants { get; set; }
        public LocationStatus Status { get; set; }
        public string MessageReponse { get; set; }
        public DateTime? DateReponse { get; set; }
        public DateTime DateCreation { get; set; }
    }
}
