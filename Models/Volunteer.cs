using System.ComponentModel.DataAnnotations;

namespace VeteranAnalyticsSystem.Models
{
    public class Volunteer
    {
        [Key]
        public int VolunteerId { get; set; }

        [Required]
        public required string FirstName { get; set; }

        [Required]
        public required string LastName { get; set; }

        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        [Required]
        public required string PhoneNumber { get; set; }
    }
}
