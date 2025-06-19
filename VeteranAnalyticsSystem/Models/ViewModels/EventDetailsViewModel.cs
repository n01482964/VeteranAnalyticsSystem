using VeteranAnalyticsSystem.Models.Core;

namespace VeteranAnalyticsSystem.Models.ViewModels
{
    public class EventDetailsViewModel
    {
        public Event Event { get; set; } = default!;
        public List<Veteran> Veterans { get; set; } = new();
        public List<Survey> MatchedSurveys { get; set; } = new();
    }
}
