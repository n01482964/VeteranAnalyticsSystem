using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace VeteranAnalyticsSystem.Models.Core
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [StringLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string LastName { get; set; } = string.Empty;

        // Email and PhoneNumber are already inherited from IdentityUser
    }
}
