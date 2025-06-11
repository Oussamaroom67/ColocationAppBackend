using ColocationAppBackend.Enums;

namespace ColocationAppBackend.DTOs.Requests
{
    public class RepondreDemandeRequest
    {
        public int DemandeId { get; set; }
        public StatutDemande NouveauStatut { get; set; } // Acceptée ou Refusée
        public string ReponseMessage { get; set; }
    }
}
