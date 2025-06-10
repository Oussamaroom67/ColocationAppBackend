using ColocationAppBackend.Enums;

namespace ColocationAppBackend.DTOs.Requests
{
    public class ChangerStatutAnnonceRequest
    {
        public int AnnonceId { get; set; }
        public AnnonceStatus NouveauStatut { get; set; }
    }
}
