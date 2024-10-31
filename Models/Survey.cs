namespace VeteranAnalyticsSystem.Models
{
    public class Survey
    {
        public int SurveyId { get; set; }
        public int ClientId { get; set; }
        public int EventId { get; set; }
        public Dictionary<string, string> Responses { get; set; } = new Dictionary<string, string>(); // Questions and answers
        public DateTime SubmissionDate { get; set; }
    }

}
