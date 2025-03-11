using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;

namespace VeteranAnalyticsSystem.Models
{
    public class Veteran
    {
        // Unique identifier generated from the email.
        [Key]
        [Required]
        [StringLength(64)]
        public string VeteranId { get; private set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public required string FirstName { get; set; }

        [Required]
        [StringLength(100)]
        public required string LastName { get; set; }

        private string _email = string.Empty;
        [Required]
        [EmailAddress]
        public required string Email 
        { 
            get => _email;
            set
            {
                _email = value;
                VeteranId = ComputeHash(_email);
            }
        }

        [Required]
        [StringLength(20)]
        public required string PhoneNumber { get; set; }

        [Required]
        [StringLength(200)]
        public required string HomeAddress { get; set; }

        [Required]
        [StringLength(100)]
        public required string City { get; set; }

        [Required]
        [StringLength(50)]
        public required string State { get; set; }

        [Required]
        [StringLength(50)]
        public required string RelationshipStatus { get; set; }

        [Required]
        [StringLength(20)]
        public required string Gender { get; set; }

        [Required]
        public DateTime DateOfBirth { get; set; }

        // Military service details.
        [Required]
        [StringLength(50)]
        public required string MilitaryServiceStatus { get; set; }  // e.g., Retired, Active, Discharged

        [Required]
        [StringLength(10)]
        public required string HighestRank { get; set; }

        [Required]
        public DateTime StartOfService { get; set; }

        public DateTime EndOfService { get; set; }

        [Required]
        [StringLength(50)]
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

        // Retreat field: stores the retreat date/location (e.g., "Amelia Island, FL - November 5-8, 2020")
        [Required]
        [StringLength(150)]
        public required string RetreatDateLocation { get; set; }

        // Navigation property for many-to-many relationship with events.
        public ICollection<Event> Events { get; set; } = new List<Event>();

        private string ComputeHash(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return string.Empty;

            using (var sha256 = SHA256.Create())
            {
                var normalizedEmail = email.Trim().ToLower();
                var bytes = Encoding.UTF8.GetBytes(normalizedEmail);
                var hash = sha256.ComputeHash(bytes);
                return BitConverter.ToString(hash).Replace("-", "").ToLower();
            }
        }
    }
}
