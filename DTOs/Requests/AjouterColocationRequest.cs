using ColocationAppBackend.Enums;

namespace ColocationAppBackend.DTOs.Requests
{
    public class AjouterColocationRequest
    {
        public string Adresse { get; set; }
        public double Budget { get; set; }
        public ColocationType Type { get; set; }
        public DateTime DateDebutDisponibilite { get; set; }
        public List<string> Preferences { get; set; }
    }
}
