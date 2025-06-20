namespace VeteranAnalyticsSystem.Models.Core;

public enum SurveyType
{
    PreRetreat = 0,
    PostRetreat = 1
}

public class Survey
{
    public int Id { get; set; }

    public int EventId { get; set; }

    public SurveyType SurveyType { get; set; }

    public DateTime? SubmissionDate { get; set; }

    public string? Email { get; set; }

    public string? SelfIdentifier { get; set; }

    public string? EmotionalConnection { get; set; }

    public string? ConflictResolution { get; set; }

    public string? PastStruggles { get; set; }

    public string? ComfortZone { get; set; }

    public string? Rating { get; set; }

    public string? ExperienceRating { get; set; }

    public string? LifeImpact { get; set; }

    public string? Recommendation { get; set; }

    public string? Feedback { get; set; }

    public required string ResponsesJson { get; set; }

    public string DisplaySubmissionDate => SubmissionDate?.ToShortDateString() ?? "N/A";
}
