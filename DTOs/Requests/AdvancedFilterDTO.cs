namespace ColocationAppBackend.DTOs.Requests
{
    public class AdvancedFilterDTO
    {
        public int Price { get; set; }
        public List<string>? PropertyType { get; set; }
        public int? Bedrooms { get; set; }
        public int? Bathrooms { get; set; }
        public List<string>? Amenities { get; set; }
    }

}
