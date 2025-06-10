namespace ColocationAppBackend.DTOs.Responses
{
    public class AnnonceSummaryDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Location { get; set; }
        public string PublishDate { get; set; }
        public string Price { get; set; }
        public int Views { get; set; }
        public string Status { get; set; }
        public string ImageUrl { get; set; }
    }
}