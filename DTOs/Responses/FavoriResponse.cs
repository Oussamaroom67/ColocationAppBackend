namespace ColocationAppBackend.DTOs.Responses
{
    public class FavoriResponse
    {
        public int Id { get; set; }
        public string Titre { get; set; }
        public decimal Prix { get; set; }
        public string Type { get; set; }
        public string Location { get; set; }
        public int Chambres { get; set; }
        public int Bathrooms { get; set; }
        public string State { get; set; }
        public string Image { get; set; }
    }
}
