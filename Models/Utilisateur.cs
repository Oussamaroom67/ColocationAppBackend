using ColocationAppBackend.Enums;

namespace ColocationAppBackend.Models
{
    public abstract class Utilisateur
    {
        protected Utilisateur() {
            ConversationsParticipant1 = new HashSet<Conversation>();
            ConversationsParticipant2 = new HashSet<Conversation>();
            SignalementsEnvoyes = new HashSet<Signalement>();
            SignalementsRecus = new HashSet<Signalement>();
            SignalementsTraites = new HashSet<Signalement>();
        }
        public int Id { get; set; }
        public string Nom { get; set; }
        public string Email { get; set; }
        public string MotDePasse { get; set; }
        public string? Telephone { get; set; }
        public string Prenom { get; set; }
        public DateTime DateInscription { get; set; }
        public UtilisateurStatus Status { get; set; } = UtilisateurStatus.Actif;
        public bool EstVerifie { get; set; }
        public DateTime DernierConnexion { get; set; }
        public DateTime DateModification { get; set; }
        public string? AvatarUrl { get; set; }
        public DateTime? LastSuspendedAt { get; set; }

        public ICollection<Conversation> ConversationsParticipant1 { get; set; }
        public ICollection<Conversation> ConversationsParticipant2 { get; set; }
        public ICollection<Signalement> SignalementsEnvoyes { get; set; }
        public ICollection<Signalement> SignalementsRecus { get; set; }
        public ICollection<Signalement> SignalementsTraites { get; set; }
    }
}
