namespace ColocationAppBackend.Models
{
    public class Logement
    {
        public int Id { get; set; }
        public string Adresse { get; set; }
        public decimal Surface { get; set; }
        public int NbChambres { get; set; }
        public string Ville { get; set; }
        public string CodePostal { get; set; }
        public string Pays { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public int? NbSallesBain { get; set; }
        public int? Etage { get; set; }
        public int? NbEtagesTotal { get; set; }
        public bool EstMeuble { get; set; }
        public bool AnimauxAutorises { get; set; }
        public bool FumeurAutorise { get; set; }
        public bool InternetInclus { get; set; }
        public bool ChargesIncluses { get; set; }
        public bool ParkingDisponible { get; set; }
        public int ProprietaireId { get; set; }
        public Proprietaire Proprietaire { get; set; }
        public Annonce Annonce { get; set; }


    }
}
