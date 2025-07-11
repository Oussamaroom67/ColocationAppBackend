namespace ColocationAppBackend.DTOs.Responses
{
    public class PropertyAnalyticsDto
    {
        public List<PropertyPerformanceDto> PropertyPerformances { get; set; }
        public double ResponseRate { get; set; }
        public double AverageResponseTimeHours { get; set; }
    }
}
