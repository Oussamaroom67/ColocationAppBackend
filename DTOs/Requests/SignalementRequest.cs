namespace ColocationAppBackend.DTOs.Requests
{
    public class SignalementRequest
    {
        public int AnnonceSignaleeId { get; set; }
        public string Motif { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        // Automatiquement déterminé côté serveur
        public int SignaleurId { get; set; }
    }
}
