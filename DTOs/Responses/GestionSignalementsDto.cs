namespace ColocationAppBackend.DTOs.Responses
{
    public class GestionSignalementsDto
    {
        public string status { get; set; }
        public string DateSignalement { get; set; }
        public string DateResolution { get; set; }
        public string signaleurName { get; set; }
        public string signaleurEmail { get; set; }
        public string description { get; set; }
        public string contentType { get; set; }
        public string UtilisateurSignaleName { get; set; }
        public string contentName { get; set; }
        public int contentId { get; set; }
        public string motif { get; set; }
        public int id { get; set; }
    }
}

