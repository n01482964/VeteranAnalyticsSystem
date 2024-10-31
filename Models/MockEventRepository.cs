// Data/MockEventRepository.cs
using System;
using System.Collections.Generic;
using VeteranAnalyticsSystem.Models;

namespace VeteranApplication.Data
{
    public static class MockEventRepository
    {
        private static List<Event> events = new List<Event>
        {
            new Event { EventId = 1, EventName = "Community Outreach", Location = "Jacksonville, FL", EventDate = new DateTime(2023, 3, 5) },
            new Event { EventId = 2, EventName = "Veteran Wellness Workshop", Location = "Savannah, GA", EventDate = new DateTime(2023, 5, 20) },
            new Event { EventId = 3, EventName = "Support Group", Location = "Atlanta, GA", EventDate = new DateTime(2023, 7, 15) },
            // Add more events if needed
        };

        public static List<Event> GetAllEvents()
        {
            return events ?? new List<Event>(); // Return an empty list if events is null
        }

        public static void AddEvent(Event eventItem)
        {
            events.Add(eventItem);
        }

        public static Event GetEventById(int id)
        {
            return events.Find(e => e.EventId == id);
        }

        public static void AddParticipantToEvent(int eventId, Veteran veteran)
        {
            var eventItem = GetEventById(eventId);
            if (eventItem != null && veteran != null)
            {
                eventItem.Participants.Add(veteran);
            }
        }
    }
}
