namespace ColocationAppBackend.DTOs.Responses.AnalyticsDTOs
{
    public class AnalyticsDTO
    {
        public double TauxOccupation { get; set; }
        public int TotalUtilisateurs { get; set; }
        public int TotalEtudiants { get; set; }
        public int TotalProprietaires { get; set; }
        public int TotalReservations { get; set; }
        public double EvolutionReservationsPourcentage { get; set; }
        public double PrixLoyerMoyen { get; set; }
        public List<ReservationMoisDto> EvolutionReservations { get; set; }
        public List<TypeLogementDto> RepartitionParType { get; set; }
        public List<VilleStatDto> RepartitionParVille { get; set; }
    }
}
