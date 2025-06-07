using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace VeteranAnalyticsSystem.Models
{
    // Define an enum to distinguish survey types.
    public enum SurveyType
    {
        PreRetreat = 0,
        PostRetreat = 1
    }

    public class Survey
    {
        // Primary key (auto-generated)
        [Key]
        public int SurveyId { get; set; }

        // Use the Email field to relate this survey to a veteran.
        [Required]
        [EmailAddress]
        [StringLength(256)]
        public string Email { get; set; } = string.Empty;

        // Optional: Capture an associated event.
        public int EventId { get; set; }

        // Enum to distinguish between pre- and post-retreat surveys.
        [Required]
        [Display(Name = "Survey Type")]
        public SurveyType SurveyType { get; set; } = SurveyType.PreRetreat;

        // Date the survey was submitted.
        public DateTime SubmissionDate { get; set; }

        /// <summary>
        /// Holds the survey questions and responses in memory as a Dictionary.
        /// This property is not directly mapped to a database column.
        /// </summary>
        [NotMapped]
        public Dictionary<string, string> Responses { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// This property stores the JSON representation of the Responses dictionary in the database.
        /// It uses System.Text.Json for serialization.
        /// </summary>
        [Column("Responses", TypeName = "nvarchar(max)")]
        public string ResponsesJson
        {
            get => JsonSerializer.Serialize(Responses);
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    Responses = new Dictionary<string, string>();
                    return;
                }

                try
                {
                    var raw = JsonSerializer.Deserialize<Dictionary<string, string>>(value);

                    // Decode any escaped unicode in the values (e.g., \u0027 => ')
                    Responses = raw?.ToDictionary(
                        kvp => kvp.Key,
                        kvp => DecodeEscapedJsonValue(kvp.Value)
                    ) ?? new Dictionary<string, string>();
                }
                catch
                {
                    Responses = new Dictionary<string, string>();
                }
            }
        }

        /// <summary>
        /// Decodes escaped unicode (e.g., \u0027) from JSON-encoded string values
        /// </summary>
        private static string DecodeEscapedJsonValue(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return input;

            try
            {
                // Wrap input in quotes to mimic full JSON string, then deserialize
                return JsonSerializer.Deserialize<string>($"\"{input}\"") ?? input;
            }
            catch
            {
                return input;
            }
        }
    }
}
