using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace VeteranAnalyticsSystem.Models.Core
{
    public class Event
    {
        public int EventId { get; set; }

        [Required]
        [StringLength(100)]
        public required string Location { get; set; }

        [Required]
        public DateTime EventDate { get; set; }

        // Navigation property: One Event can have many Veterans
        public ICollection<Veteran> Veterans { get; set; } = new List<Veteran>();
    }
}
