using ColocationAppBackend.Enums;

namespace ColocationAppBackend.DTOs.Requests
{
    public class ChangeStatusDto
    {
        public int DemandeId { get; set; }
        public LocationStatus NouveauStatus { get; set; }
        public string MessageReponse { get; set; } = "";
    }
}
