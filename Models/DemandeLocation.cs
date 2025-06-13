using ColocationAppBackend.Enums;

namespace ColocationAppBackend.Models
{
    public class DemandeLocation
    {
        public int Id {  get; set; }
        public int AnnonceId { get; set; }
        public Annonce Annonce { get; set; }
        public int EtudiantId { get; set; }
        public Etudiant Etudiant { get; set; }
        public string Message { get; set; } = "";
        public DateTime DateEmmenagement { get; set; }
        public int DureeSejour { get; set; }
        public int NbOccupants { get; set; }
        public LocationStatus status { get; set; } = LocationStatus.EnAttente;
        public string MessageReponse { get; set; } = "";
        public DateTime DateReponse {  get; set; }
        public DateTime DateCreation {  get; set; }
    }
}
