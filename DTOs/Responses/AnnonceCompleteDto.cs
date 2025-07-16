using ColocationAppBackend.Enums;

namespace ColocationAppBackend.DTOs.Responses
{
    public class AnnonceCompleteDto
    {
        // Données de l'annonce
        public int AnnonceId { get; set; }
        public string Titre { get; set; }
        public string Description { get; set; }
        public decimal Prix { get; set; }
        public decimal Caution { get; set; }
        public decimal Charges { get; set; }
        public DateTime DisponibleDe { get; set; }
        public DateTime DisponibleJusqu { get; set; }
        public int DureeMinMois { get; set; }
        public int OccupantsMax { get; set; }
        public AnnonceStatus StatutAnnonce { get; set; }
        public int NbVues { get; set; }
        public DateTime DateModification { get; set; }
        public VerificationLogementStatut StatutVerification { get; set; }

        // Données du logement
        public int LogementId { get; set; }
        public string Adresse { get; set; }
        public decimal Surface { get; set; }
        public int NbChambres { get; set; }
        public string Ville { get; set; }
        public string CodePostal { get; set; }
        public string Pays { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public int NbSallesBain { get; set; }
        public int? Etage { get; set; }
        public int? NbEtagesTotal { get; set; }
        public bool EstMeuble { get; set; }
        public bool AnimauxAutorises { get; set; }
        public bool FumeurAutorise { get; set; }
        public bool InternetInclus { get; set; }
        public bool ChargesIncluses { get; set; }
        public bool ParkingDisponible { get; set; }
        public LogementStatus StatutLogement { get; set; }
        public string TypeLogement { get; set; }

        // Données du propriétaire
        public int ProprietaireId { get; set; }
        public string ProprietaireNom { get; set; }
        public string ProprietairePrenom { get; set; }
        public string ProprietaireEmail { get; set; }
        public string ProprietaireTelephone { get; set; }

        // Listes des relations
        public List<PhotoInfo> Photos { get; set; } = new List<PhotoInfo>();
       
    }
}
