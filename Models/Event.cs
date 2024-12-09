using System;
using System.Collections.Generic;

namespace VeteranAnalyticsSystem.Models
{
    public class Event
    {
        public int EventId { get; set; }
        public string EventName { get; set; }
        public string Location { get; set; }
        public DateTime EventDate { get; set; }

        // Navigation property for related Veterans
        public List<Veteran> Participants { get; set; } = new List<Veteran>();
    }
}
