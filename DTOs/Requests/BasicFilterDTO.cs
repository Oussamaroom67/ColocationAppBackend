namespace ColocationAppBackend.DTOs.Requests
{
    public class BasicFilterDTO
    {
        public string? Ville { get; set; }
        public string? PropertyType { get; set; }
        public int? MinPrice { get; set; }
    }
}
