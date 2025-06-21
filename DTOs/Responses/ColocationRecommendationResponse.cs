using ColocationAppBackend.Enums;

namespace ColocationAppBackend.DTOs.Responses
{
    public class ColocationRecommendationResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string School { get; set; }
        public string Budget { get; set; }
        public string MoveInDate { get; set; }
        public string PreferredZone { get; set; }
        public ColocationType Type { get; set; }
        public float RecommendationScore { get; set; }
        public bool IsRecommended { get; set; }
        public List<string> Preferences { get; set; }
        public string AvatarUrl { get; set; }
    }
}
