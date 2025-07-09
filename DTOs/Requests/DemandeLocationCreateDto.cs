namespace ColocationAppBackend.DTOs.Requests
{
    public class DemandeLocationCreateDto
    {
        public int AnnonceId { get; set; }
        public string Message { get; set; } = "";
        public DateTime DateEmmenagement { get; set; }
        public int DureeSejour { get; set; }
        public int NbOccupants { get; set; }
        public int EtudiantId { get; set; }
    }
}
