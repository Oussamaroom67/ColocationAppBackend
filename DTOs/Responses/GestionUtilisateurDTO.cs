using ColocationAppBackend.Enums;

namespace ColocationAppBackend.DTOs.Responses
{
    public class GestionUtilisateurDTO
    {
        public int id {  get; set; }
        public string Nom { get; set; }
        public string Email { get; set; }
        public string Type{ get; set; }
        public UtilisateurStatus Statut { get; set; }
        public bool EstVerifie { get; set; }
        public string DateInscription { get; set; }
        public string avatarUrl { get; set; }

    }
}
