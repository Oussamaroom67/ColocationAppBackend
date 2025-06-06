using System.ComponentModel.DataAnnotations.Schema;
using ColocationAppBackend.Enums;
namespace ColocationAppBackend.Models
{
    public class DemandeColocation
    {
        public int Id { get; set; }
        public decimal? Budget { get; set; }
        public string? Adresse { get; set; }
        public string Message { get; set; } = string.Empty;
        public DateTime DateEmmenagement { get; set; }
        public List<string> Preferences { get; set; }
        public StatutDemande Statut { get; set; } = StatutDemande.EnAttente;
        public int EtudiantId { get; set; }
        public Etudiant Etudiant { get; set; }
    }
}
