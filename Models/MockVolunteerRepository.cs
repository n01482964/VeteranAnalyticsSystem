using System.Collections.Generic;
using System.Linq;
using VeteranAnalyticsSystem.Models;

namespace VeteranAnalyticsSystem.Data
{
    public static class MockVolunteerRepository
    {
        private static List<Volunteer> volunteers = new List<Volunteer>
        {
            new Volunteer { VolunteerId = 1, FirstName = "Alice", LastName = "Johnson", Email = "alice.johnson@example.com", PhoneNumber = "123-456-7890" },
            new Volunteer { VolunteerId = 2, FirstName = "Bob", LastName = "Smith", Email = "bob.smith@example.com", PhoneNumber = "234-567-8901" },
            new Volunteer { VolunteerId = 3, FirstName = "Carol", LastName = "Davis", Email = "carol.davis@example.com", PhoneNumber = "345-678-9012" },
            new Volunteer { VolunteerId = 4, FirstName = "David", LastName = "Miller", Email = "david.miller@example.com", PhoneNumber = "456-789-0123" },
            new Volunteer { VolunteerId = 5, FirstName = "Eve", LastName = "Williams", Email = "eve.williams@example.com", PhoneNumber = "567-890-1234" }
        };

        public static List<Volunteer> GetAllVolunteers()
        {
            return volunteers;
        }

        public static void AddVolunteer(Volunteer volunteer)
        {
            volunteer.VolunteerId = volunteers.Count > 0 ? volunteers.Max(v => v.VolunteerId) + 1 : 1;
            volunteers.Add(volunteer);
        }

        public static Volunteer GetVolunteerById(int id)
        {
            return volunteers.FirstOrDefault(v => v.VolunteerId == id);
        }

        public static void UpdateVolunteer(Volunteer volunteer)
        {
            var existingVolunteer = GetVolunteerById(volunteer.VolunteerId);
            if (existingVolunteer != null)
            {
                existingVolunteer.FirstName = volunteer.FirstName;
                existingVolunteer.LastName = volunteer.LastName;
                existingVolunteer.Email = volunteer.Email;
                existingVolunteer.PhoneNumber = volunteer.PhoneNumber;
            }
        }

        public static void DeleteVolunteer(int id)
        {
            var volunteer = GetVolunteerById(id);
            if (volunteer != null)
            {
                volunteers.Remove(volunteer);
            }
        }
    }
}
