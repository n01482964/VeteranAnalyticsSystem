using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace VeteranAnalyticsSystem.Models
{
    public class Event
    {
        public int EventId { get; set; }

        [Required]
        public required string EventName { get; set; }

        [Required]
        public required string Location { get; set; } = string.Empty;

        public DateTime EventDate { get; set; }

        // Navigation property for many-to-many relation with veterans.
        public ICollection<Veteran> Participants { get; set; } = new List<Veteran>();
    }
}
