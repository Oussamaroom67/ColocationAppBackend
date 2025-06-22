using ColocationAppBackend.Enums;

namespace ColocationAppBackend.DTOs.Responses
{
    public class ColocationFiltreeResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string School { get; set; }
        public decimal Budget { get; set; }
        public string MoveInDate { get; set; }
        public string PreferredZone { get; set; }
        public  ColocationType Type { get; set; }
        public List<string> Preferences { get; set; }
    }
}
