namespace ColocationAppBackend.DTOs.Responses
{
    public class DashboardResponse
    {
        public int TotalUsers { get; set; }
        public string UserChange { get; set; }

        public int TotalProperties { get; set; }
        public string PropertyChange { get; set; }

        public int MessagesToday { get; set; }
        public string MessageChange { get; set; }

        public int PendingReports { get; set; }
        public int NewReportsToday { get; set; }

        public int UnverifiedUsers { get; set; }

        public UserDistributionDTO UserDistribution { get; set; }
        public List<ActivityDTO> LastActivities { get; set; }
        public List<RecentReportDTO> RecentReports { get; set; }
    }
}
