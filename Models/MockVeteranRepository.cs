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
            new Veteran { VeteranId = 1, FirstName = "John", LastName = "Doe", Gender = "Male", DateOfBirth = new DateTime(1985, 5, 10), BranchOfService = "Army", Condition = "PTSD", DateOfEntry = new DateTime(2023, 1, 15) },
            new Veteran { VeteranId = 2, FirstName = "Jane", LastName = "Smith", Gender = "Female", DateOfBirth = new DateTime(1990, 8, 20), BranchOfService = "Navy", Condition = "Amputee", DateOfEntry = new DateTime(2023, 2, 18) },
            new Veteran { VeteranId = 3, FirstName = "Alex", LastName = "Johnson", Gender = "Non-binary", DateOfBirth = new DateTime(1987, 3, 14), BranchOfService = "Marine Corps", Condition = "PTSD", DateOfEntry = new DateTime(2023, 4, 2) },
            // Add more veterans if needed
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
