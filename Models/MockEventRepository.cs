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
new Event { EventId = 4, EventName = "Volunteer Drive", Location = "Birmingham, AL", EventDate = new DateTime(2023, 1, 20) },
new Event { EventId = 5, EventName = "Community Outreach", Location = "Miami, FL", EventDate = new DateTime(2023, 3, 30) },
new Event { EventId = 6, EventName = "Financial Planning Session", Location = "Miami, FL", EventDate = new DateTime(2023, 8, 22) },
new Event { EventId = 7, EventName = "Volunteer Drive", Location = "Columbus, GA", EventDate = new DateTime(2023, 2, 7) },
new Event { EventId = 8, EventName = "Volunteer Drive", Location = "Birmingham, AL", EventDate = new DateTime(2023, 12, 24) },
new Event { EventId = 9, EventName = "Veteran Wellness Workshop", Location = "Miami, FL", EventDate = new DateTime(2023, 11, 4) },
new Event { EventId = 10, EventName = "Community Outreach", Location = "Charlotte, NC", EventDate = new DateTime(2023, 8, 31) },
new Event { EventId = 11, EventName = "Leadership Seminar", Location = "Orlando, FL", EventDate = new DateTime(2023, 9, 10) },
new Event { EventId = 12, EventName = "Fitness Bootcamp", Location = "Savannah, GA", EventDate = new DateTime(2023, 7, 22) },
new Event { EventId = 13, EventName = "Family Day", Location = "Nashville, TN", EventDate = new DateTime(2023, 6, 15) },
new Event { EventId = 14, EventName = "Mental Health Awareness", Location = "Atlanta, GA", EventDate = new DateTime(2023, 4, 18) },
new Event { EventId = 15, EventName = "Job Fair", Location = "Jacksonville, FL", EventDate = new DateTime(2023, 2, 24) },
new Event { EventId = 16, EventName = "Community Outreach", Location = "Tampa, FL", EventDate = new DateTime(2023, 10, 5) },
new Event { EventId = 17, EventName = "Support Group", Location = "Orlando, FL", EventDate = new DateTime(2023, 11, 12) },
new Event { EventId = 18, EventName = "Veteran Appreciation Banquet", Location = "Miami, FL", EventDate = new DateTime(2023, 12, 14) },
new Event { EventId = 19, EventName = "Leadership Seminar", Location = "Birmingham, AL", EventDate = new DateTime(2023, 3, 25) },
new Event { EventId = 20, EventName = "Volunteer Recruitment Drive", Location = "Savannah, GA", EventDate = new DateTime(2023, 5, 11) },
new Event { EventId = 21, EventName = "Fitness Bootcamp", Location = "Jacksonville, FL", EventDate = new DateTime(2023, 8, 17) },
new Event { EventId = 22, EventName = "Job Fair", Location = "Charlotte, NC", EventDate = new DateTime(2023, 6, 21) },
new Event { EventId = 23, EventName = "Community Outreach", Location = "Orlando, FL", EventDate = new DateTime(2023, 7, 13) },
new Event { EventId = 24, EventName = "Family Day", Location = "Columbus, GA", EventDate = new DateTime(2023, 9, 8) },
new Event { EventId = 25, EventName = "Mental Health Awareness", Location = "Miami, FL", EventDate = new DateTime(2023, 10, 23) },
new Event { EventId = 26, EventName = "Veteran Wellness Workshop", Location = "Tampa, FL", EventDate = new DateTime(2023, 12, 3) },
new Event { EventId = 27, EventName = "Support Group", Location = "Savannah, GA", EventDate = new DateTime(2023, 11, 15) },
new Event { EventId = 28, EventName = "Leadership Seminar", Location = "Atlanta, GA", EventDate = new DateTime(2023, 5, 19) },
new Event { EventId = 29, EventName = "Volunteer Drive", Location = "Columbus, GA", EventDate = new DateTime(2023, 2, 10) },
new Event { EventId = 30, EventName = "Community Outreach", Location = "Orlando, FL", EventDate = new DateTime(2023, 8, 6) },
new Event { EventId = 31, EventName = "Family Day", Location = "Nashville, TN", EventDate = new DateTime(2023, 6, 27) },
new Event { EventId = 32, EventName = "Fitness Bootcamp", Location = "Savannah, GA", EventDate = new DateTime(2023, 4, 9) },
new Event { EventId = 33, EventName = "Job Fair", Location = "Jacksonville, FL", EventDate = new DateTime(2023, 9, 20) },
new Event { EventId = 34, EventName = "Mental Health Awareness", Location = "Birmingham, AL", EventDate = new DateTime(2023, 7, 15) },
new Event { EventId = 35, EventName = "Veteran Wellness Workshop", Location = "Charlotte, NC", EventDate = new DateTime(2023, 5, 24) },
new Event { EventId = 36, EventName = "Leadership Seminar", Location = "Miami, FL", EventDate = new DateTime(2023, 3, 12) }

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
