using System.Collections.Generic;
using System.Linq;
using VeteranAnalyticsSystem.Models;

namespace VeteranAnalyticsSystem.Data
{
    public static class MockVolunteerRepository
    {
        private static List<Volunteer> volunteers = new List<Volunteer>
        {
            new Volunteer { VolunteerId = 6, FirstName = "Frank", LastName = "Brown", Email = "frank.brown@example.com", PhoneNumber = "678-901-2345" },
            new Volunteer { VolunteerId = 7, FirstName = "Grace", LastName = "Jones", Email = "grace.jones@example.com", PhoneNumber = "789-012-3456" },
            new Volunteer { VolunteerId = 8, FirstName = "Hannah", LastName = "Garcia", Email = "hannah.garcia@example.com", PhoneNumber = "890-123-4567" },
            new Volunteer { VolunteerId = 9, FirstName = "Isaac", LastName = "Martinez", Email = "isaac.martinez@example.com", PhoneNumber = "901-234-5678" },
            new Volunteer { VolunteerId = 10, FirstName = "Julia", LastName = "Rodriguez", Email = "julia.rodriguez@example.com", PhoneNumber = "012-345-6789" },
            new Volunteer { VolunteerId = 11, FirstName = "Kevin", LastName = "Wilson", Email = "kevin.wilson@example.com", PhoneNumber = "123-456-7891" },
            new Volunteer { VolunteerId = 12, FirstName = "Laura", LastName = "Anderson", Email = "laura.anderson@example.com", PhoneNumber = "234-567-8902" },
            new Volunteer { VolunteerId = 13, FirstName = "Mark", LastName = "Thomas", Email = "mark.thomas@example.com", PhoneNumber = "345-678-9013" },
            new Volunteer { VolunteerId = 14, FirstName = "Nina", LastName = "Taylor", Email = "nina.taylor@example.com", PhoneNumber = "456-789-0124" },
            new Volunteer { VolunteerId = 15, FirstName = "Oscar", LastName = "Moore", Email = "oscar.moore@example.com", PhoneNumber = "567-890-1235" },
            new Volunteer { VolunteerId = 16, FirstName = "Patricia", LastName = "Jackson", Email = "patricia.jackson@example.com", PhoneNumber = "678-901-2346" },
    new Volunteer { VolunteerId = 17, FirstName = "Quentin", LastName = "Martin", Email = "quentin.martin@example.com", PhoneNumber = "789-012-3457" },
new Volunteer { VolunteerId = 18, FirstName = "Rachel", LastName = "Lee", Email = "rachel.lee@example.com", PhoneNumber = "890-123-4568" },
new Volunteer { VolunteerId = 19, FirstName = "Samuel", LastName = "Perez", Email = "samuel.perez@example.com", PhoneNumber = "901-234-5679" },
new Volunteer { VolunteerId = 20, FirstName = "Tina", LastName = "White", Email = "tina.white@example.com", PhoneNumber = "012-345-6780" },
new Volunteer { VolunteerId = 21, FirstName = "Ursula", LastName = "Harris", Email = "ursula.harris@example.com", PhoneNumber = "123-456-7892" },
new Volunteer { VolunteerId = 22, FirstName = "Victor", LastName = "Clark", Email = "victor.clark@example.com", PhoneNumber = "234-567-8903" },
new Volunteer { VolunteerId = 23, FirstName = "Wendy", LastName = "Young", Email = "wendy.young@example.com", PhoneNumber = "345-678-9014" },
new Volunteer { VolunteerId = 24, FirstName = "Xavier", LastName = "Lopez", Email = "xavier.lopez@example.com", PhoneNumber = "456-789-0125" },
new Volunteer { VolunteerId = 25, FirstName = "Yvonne", LastName = "Hill", Email = "yvonne.hill@example.com", PhoneNumber = "567-890-1236" },
new Volunteer { VolunteerId = 26, FirstName = "Zachary", LastName = "Scott", Email = "zachary.scott@example.com", PhoneNumber = "678-901-2347" },
new Volunteer { VolunteerId = 27, FirstName = "Amber", LastName = "Adams", Email = "amber.adams@example.com", PhoneNumber = "789-012-3458" },
new Volunteer { VolunteerId = 28, FirstName = "Brian", LastName = "Nelson", Email = "brian.nelson@example.com", PhoneNumber = "890-123-4569" },
new Volunteer { VolunteerId = 29, FirstName = "Cynthia", LastName = "Carter", Email = "cynthia.carter@example.com", PhoneNumber = "901-234-5670" },
new Volunteer { VolunteerId = 30, FirstName = "Derek", LastName = "Mitchell", Email = "derek.mitchell@example.com", PhoneNumber = "012-345-6781" }

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
