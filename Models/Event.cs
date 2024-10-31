namespace VeteranAnalyticsSystem.Models
{
    public class Event
    {
        public int EventId { get; set; }
        public string EventName { get; set; } // e.g., Workshop, Lecture, etc.
        public string Location { get; set; } // e.g., Jacksonville, GA
        public DateTime EventDate { get; set; }
        public List<Veteran> Participants { get; set; } = new List<Veteran>(); // List of participants
    }

}
