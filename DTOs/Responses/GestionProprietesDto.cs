namespace ColocationAppBackend.DTOs.Responses
{
    public class GestionProprietesDto
    {
        public int id {  get; set; }
        public string nom { get; set; }
        public string adresse { get; set; }
        public string nomProprietaire { get; set; }
        public string type { get; set; }
        public decimal prix { get; set; }
        public string statut {  get; set; }
        public string dateAjout { get; set; }
    }
}
