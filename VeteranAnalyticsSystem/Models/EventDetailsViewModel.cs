namespace VeteranAnalyticsSystem.Models
{
    public class EventDetailsViewModel
    {
        public Event Event { get; set; } = default!;
        public List<Veteran> Veterans { get; set; } = new();
        public List<Survey> MatchedSurveys { get; set; } = new();
    }
}
