namespace ColocationAppBackend.DTOs.Requests
{
    public class AdvancedFilterDTO
    {
        public int? Price { get; set; }
        public List<string>? PropertyType { get; set; }
        public string Bedrooms { get; set; }
        public string Bathrooms { get; set; }
        public List<string>? Amenities { get; set; }
    }

}
