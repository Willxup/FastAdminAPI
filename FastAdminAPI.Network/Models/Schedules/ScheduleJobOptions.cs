namespace FastAdminAPI.Network.Models.Schedules
{
    public class ScheduleJobOptions
    {
        public bool IsEnable { get; set; }
        public string JobName { get; set; }
        public string Frequency { get; set; }
        public string Minute { get; set; }
        public string Hour { get; set; }
        public string Day { get; set; }
        public string DayOfWeek { get; set; }
        public string Month { get; set; }
        public string Description { get; set; }
    }
}
