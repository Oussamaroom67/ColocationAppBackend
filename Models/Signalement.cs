using ColocationAppBackend.Enums;

namespace ColocationAppBackend.Models
{
    public class Signalement
    {
        public int Id { get; set; }

        public string Motif { get; set; } = string.Empty;
        public string? Description { get; set; }

        public DateTime DateSignalement { get; set; } = DateTime.UtcNow;

        // Qui a signalé
        public int SignaleurId { get; set; }
        public Utilisateur Signaleur { get; set; }

        // Contenu ciblé
        public int UtilisateurSignaleId { get; set; }
        public Utilisateur UtilisateurSignale { get; set; }

        public int AnnonceSignaleeId { get; set; }
        public Annonce? AnnonceSignalee { get; set; }
        public SignalementType Statut { get; set; } = SignalementType.EnAttente;
        public string? NotesAdmin { get; set; }

        public int? ResoluParId { get; set; }
        public Utilisateur ResoluPar { get; set; }

        public DateTime? DateResolution { get; set; }
        public DateTime DateModification { get; set; } = DateTime.UtcNow;
    }
}
