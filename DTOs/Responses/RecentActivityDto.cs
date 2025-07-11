namespace ColocationAppBackend.DTOs.Responses
{
    public class RecentActivityDto
    {
        public int Id { get; set; }
        public string Type { get; set; } // message, view, update
        public string Title { get; set; }
        public string Description { get; set; }
        public string Time { get; set; }
        public DateTime CreatedAt { get; set; }

    }
}
