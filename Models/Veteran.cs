using System;
using System.ComponentModel.DataAnnotations;

namespace VeteranAnalyticsSystem.Models
{
    public class Veteran
    {
        [Key]
        [StringLength(64)]
        public string VeteranId { get; set; } = Guid.NewGuid().ToString();

        [Required, StringLength(100)]
        public required string FirstName { get; set; }

        [Required, StringLength(100)]
        public required string LastName { get; set; }

        [Required, EmailAddress]
        public required string Email { get; set; }

        [Required, StringLength(20)]
        public required string PhoneNumber { get; set; }

        [Required, StringLength(200)]
        public required string HomeAddress { get; set; }

        [Required, StringLength(100)]
        public required string City { get; set; }

        [Required, StringLength(50)]
        public required string State { get; set; }

        [Required, StringLength(50)]
        public required string RelationshipStatus { get; set; }

        [Required, StringLength(20)]
        public required string Gender { get; set; }

        [Required]
        public DateTime DateOfBirth { get; set; }

        [Required]
        public required string MilitaryServiceStatus { get; set; }

        public required string HighestRank { get; set; }

        [Required]
        public DateTime StartOfService { get; set; }

        public DateTime EndOfService { get; set; }

        public required string BranchOfService { get; set; }

        public int NumberOfDeployments { get; set; }

        [Required]
        public required string DeploymentDetails { get; set; }

        [Required]
        public required string HealthConcerns { get; set; }

        [Required]
        public required string AdditionalHealthInfo { get; set; }

        [Required]
        public required string PhysicalLimitations { get; set; }

        // ⭐ Optional Star Rating: 1–5 or null for "unrated"
        [Range(1, 5)]
        public int? StarRating { get; set; }

        // Foreign key to Event
        public int EventId { get; set; }
        public Event? Event { get; set; }
    }
}
