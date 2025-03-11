using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace VeteranAnalyticsSystem.Models
{
    public class Survey
    {
        [Key]
        public int SurveyId { get; set; }

        // Foreign key linking to the Veteran (generated from email)
        [Required]
        public required string VeteranId { get; set; } = string.Empty;
        public required Veteran Veteran { get; set; }

        // Link to an event (assumed required)
        public int EventId { get; set; }
        public required Event Event { get; set; }

        // This property is used in code but not mapped by EF Core.
        [NotMapped]
        public Dictionary<string, string> Responses { get; set; } = new Dictionary<string, string>();

        // Mapped property that stores the JSON representation of the responses.
        [Column("Responses", TypeName = "nvarchar(max)")]
        public string ResponsesJson
        {
            get => JsonSerializer.Serialize(Responses);
            set => Responses = string.IsNullOrWhiteSpace(value)
                ? new Dictionary<string, string>()
                : JsonSerializer.Deserialize<Dictionary<string, string>>(value) ?? new Dictionary<string, string>();
        }

        public DateTime SubmissionDate { get; set; }

        // Helper method to add or update a response.
        public void AddResponse(string question, string answer)
        {
            Responses[question] = answer;
        }
    }
}
