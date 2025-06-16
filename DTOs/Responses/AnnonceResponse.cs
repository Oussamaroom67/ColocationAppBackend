using ColocationAppBackend.DTOs.Requests;

namespace ColocationAppBackend.DTOs.Responses
{
    public class AnnonceResponse
    {
        public int AnnonceId { get; set; }
        public int LogementId { get; set; }
        public string Title { get; set; }
        public decimal Prix { get; set; }
        public string Type { get; set; }
        public string Ville { get; set; }
        public List<PhotoDto> Photos { get; set; }
        public int Beds { get; set; }
        public int Baths { get; set; }
    }

}

