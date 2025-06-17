namespace ColocationAppBackend.DTOs.Responses
{
    public class DemandeColocationRecuDto
    {
        public int Id { get; set; }
        public string NomDemandeur { get; set; }
        public string PrenomDemandeur { get; set; } 
        public string UniversiteDemandeur { get; set; }
        public DateTime DateEmmenagement { get; set; }
        public int ColocationId { get; set; }
        public decimal? BudgetDemandeur { get; set; }
        public string? Adresse { get; set; }
    }
}
