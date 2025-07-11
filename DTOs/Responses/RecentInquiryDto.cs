namespace ColocationAppBackend.DTOs.Responses
{
    public class RecentInquiryDto
    {
        public int Id { get; set; }
        public string NomEtudiant { get; set; }
        public string InitialeEtudiant { get; set; }
        public string TitrePropriete { get; set; }
        public string Message { get; set; }
        public DateTime DateCreation { get; set; }
        public bool EstRepondu { get; set; }
        public string Statut { get; set; }
        public int AnnonceId { get; set; }
        public int EtudiantId { get; set; }

        // Propriété calculée pour l'affichage de la date
        public string HeureAffichage
        {
            get
            {
                var now = DateTime.Now;
                var diff = now - DateCreation;

                if (diff.Days == 0)
                {
                    return $"Aujourd'hui, {DateCreation:HH:mm}";
                }
                else if (diff.Days == 1)
                {
                    return "Hier";
                }
                else if (diff.Days <= 7)
                {
                    return $"Il y a {diff.Days} jour{(diff.Days > 1 ? "s" : "")}";
                }
                else
                {
                    return DateCreation.ToString("dd/MM/yyyy");
                }
            }
        }
    }
}
