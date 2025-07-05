namespace ColocationAppBackend.DTOs.Responses
{
    public class FeaturedPropDto
    {
        public string Image { get; set; } = "";
        public string Titre { get; set; } = "";
        public string Adresse { get; set; } = "";
        public int NbSallesBain { get; set; }
        public int NbChambres { get; set; }
    }
}
