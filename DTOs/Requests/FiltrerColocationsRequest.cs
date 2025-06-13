using ColocationAppBackend.Enums;

namespace ColocationAppBackend.DTOs.Requests
{
    public class FiltrerColocationsRequest
    {
        public decimal? BudgetMax { get; set; }
        public List<string>? Preferences { get; set; }
    }
}
