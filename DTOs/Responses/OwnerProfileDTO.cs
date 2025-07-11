using ColocationAppBackend.Models;

namespace ColocationAppBackend.DTOs.Responses
{
    public class OwnerProfileDTO
    {
        public string Nom {  get; set; }
        public List<AvisResponseDTO> Avis {  get; set; }= new List<AvisResponseDTO>();
        public int NmbProprietes { get; set; }
        public string Adresse { get; set; }
        public string pays { get; set; }
        public string Ville { get; set; }
        public string AvatarProp { get; set; }
        public double Note {  get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
    }
}
