// Data/MockVeteranRepository.cs
using System;
using System.Collections.Generic;
using VeteranAnalyticsSystem.Models;

namespace VeteranApplication.Data
{
    public static class MockVeteranRepository
    {
        private static List<Veteran> veterans = new List<Veteran>
        {
            new Veteran { VeteranId = 4, FirstName = "Michael", LastName = "Brown", Gender = "Male", DateOfBirth = new DateTime(1982, 12, 5), BranchOfService = "Air Force", Condition = "Hearing Loss", DateOfEntry = new DateTime(2023, 3, 10) },
new Veteran { VeteranId = 5, FirstName = "Emily", LastName = "Davis", Gender = "Female", DateOfBirth = new DateTime(1993, 7, 21), BranchOfService = "Army", Condition = "TBI", DateOfEntry = new DateTime(2023, 4, 15) },
new Veteran { VeteranId = 6, FirstName = "Chris", LastName = "Miller", Gender = "Male", DateOfBirth = new DateTime(1978, 2, 28), BranchOfService = "Navy", Condition = "Amputee", DateOfEntry = new DateTime(2023, 5, 5) },
new Veteran { VeteranId = 7, FirstName = "Sophia", LastName = "Garcia", Gender = "Female", DateOfBirth = new DateTime(1995, 9, 11), BranchOfService = "Marine Corps", Condition = "PTSD", DateOfEntry = new DateTime(2023, 6, 1) },
new Veteran { VeteranId = 8, FirstName = "Daniel", LastName = "Martinez", Gender = "Male", DateOfBirth = new DateTime(1986, 11, 30), BranchOfService = "Army", Condition = "Anxiety", DateOfEntry = new DateTime(2023, 7, 8) },
new Veteran { VeteranId = 9, FirstName = "Olivia", LastName = "Hernandez", Gender = "Female", DateOfBirth = new DateTime(1991, 6, 15), BranchOfService = "Air Force", Condition = "Depression", DateOfEntry = new DateTime(2023, 8, 12) },
new Veteran { VeteranId = 10, FirstName = "Ethan", LastName = "Wilson", Gender = "Male", DateOfBirth = new DateTime(1989, 4, 18), BranchOfService = "Navy", Condition = "PTSD", DateOfEntry = new DateTime(2023, 9, 20) },
new Veteran { VeteranId = 11, FirstName = "Isabella", LastName = "Lopez", Gender = "Female", DateOfBirth = new DateTime(1983, 10, 22), BranchOfService = "Marine Corps", Condition = "Amputee", DateOfEntry = new DateTime(2023, 10, 5) },
new Veteran { VeteranId = 12, FirstName = "James", LastName = "Clark", Gender = "Male", DateOfBirth = new DateTime(1988, 3, 9), BranchOfService = "Army", Condition = "TBI", DateOfEntry = new DateTime(2023, 11, 7) },
new Veteran { VeteranId = 13, FirstName = "Mia", LastName = "Rodriguez", Gender = "Female", DateOfBirth = new DateTime(1994, 5, 14), BranchOfService = "Air Force", Condition = "Hearing Loss", DateOfEntry = new DateTime(2023, 12, 3) },
new Veteran { VeteranId = 14, FirstName = "Noah", LastName = "Lewis", Gender = "Male", DateOfBirth = new DateTime(1980, 1, 25), BranchOfService = "Navy", Condition = "Depression", DateOfEntry = new DateTime(2023, 1, 15) },
new Veteran { VeteranId = 15, FirstName = "Ava", LastName = "Walker", Gender = "Female", DateOfBirth = new DateTime(1992, 8, 8), BranchOfService = "Marine Corps", Condition = "PTSD", DateOfEntry = new DateTime(2023, 2, 10) },
new Veteran { VeteranId = 16, FirstName = "Liam", LastName = "Hall", Gender = "Male", DateOfBirth = new DateTime(1984, 6, 29), BranchOfService = "Army", Condition = "Anxiety", DateOfEntry = new DateTime(2023, 3, 25) },
new Veteran { VeteranId = 17, FirstName = "Charlotte", LastName = "Allen", Gender = "Female", DateOfBirth = new DateTime(1987, 9, 19), BranchOfService = "Air Force", Condition = "TBI", DateOfEntry = new DateTime(2023, 4, 8) },
new Veteran { VeteranId = 18, FirstName = "William", LastName = "Young", Gender = "Male", DateOfBirth = new DateTime(1979, 12, 12), BranchOfService = "Navy", Condition = "Amputee", DateOfEntry = new DateTime(2023, 5, 18) },
new Veteran { VeteranId = 19, FirstName = "Ella", LastName = "King", Gender = "Female", DateOfBirth = new DateTime(1996, 11, 5), BranchOfService = "Marine Corps", Condition = "PTSD", DateOfEntry = new DateTime(2023, 6, 15) },
new Veteran { VeteranId = 20, FirstName = "Benjamin", LastName = "Scott", Gender = "Male", DateOfBirth = new DateTime(1981, 2, 28), BranchOfService = "Army", Condition = "Hearing Loss", DateOfEntry = new DateTime(2023, 7, 9) },
new Veteran { VeteranId = 21, FirstName = "Amelia", LastName = "Green", Gender = "Female", DateOfBirth = new DateTime(1990, 4, 4), BranchOfService = "Air Force", Condition = "Anxiety", DateOfEntry = new DateTime(2023, 8, 22) },
new Veteran { VeteranId = 22, FirstName = "Mason", LastName = "Baker", Gender = "Male", DateOfBirth = new DateTime(1983, 7, 7), BranchOfService = "Navy", Condition = "Depression", DateOfEntry = new DateTime(2023, 9, 2) },
new Veteran { VeteranId = 23, FirstName = "Harper", LastName = "Nelson", Gender = "Female", DateOfBirth = new DateTime(1991, 5, 30), BranchOfService = "Marine Corps", Condition = "TBI", DateOfEntry = new DateTime(2023, 10, 10) },
new Veteran { VeteranId = 24, FirstName = "Elijah", LastName = "Carter", Gender = "Male", DateOfBirth = new DateTime(1985, 10, 15), BranchOfService = "Army", Condition = "Amputee", DateOfEntry = new DateTime(2023, 11, 20) },
new Veteran { VeteranId = 25, FirstName = "Lily", LastName = "Mitchell", Gender = "Female", DateOfBirth = new DateTime(1989, 3, 3), BranchOfService = "Air Force", Condition = "PTSD", DateOfEntry = new DateTime(2023, 12, 6) },
new Veteran { VeteranId = 26, FirstName = "Lucas", LastName = "Perez", Gender = "Male", DateOfBirth = new DateTime(1982, 6, 10), BranchOfService = "Navy", Condition = "Anxiety", DateOfEntry = new DateTime(2023, 1, 25) },
new Veteran { VeteranId = 27, FirstName = "Grace", LastName = "Roberts", Gender = "Female", DateOfBirth = new DateTime(1993, 12, 20), BranchOfService = "Marine Corps", Condition = "Depression", DateOfEntry = new DateTime(2023, 2, 12) },
new Veteran { VeteranId = 28, FirstName = "Henry", LastName = "Turner", Gender = "Male", DateOfBirth = new DateTime(1987, 9, 28), BranchOfService = "Army", Condition = "Hearing Loss", DateOfEntry = new DateTime(2023, 3, 7) }

        };

        public static List<Veteran> GetAllVeterans()
        {
            return veterans ?? new List<Veteran>(); // Return an empty list if veterans is null
        }

        public static void AddVeteran(Veteran veteran)
        {
            veterans.Add(veteran);
        }

        public static Veteran GetVeteranById(int id)
        {
            return veterans.Find(v => v.VeteranId == id);
        }
    }
}
