// Controllers/VeteranController.cs
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using VeteranAnalyticsSystem.Models;
using VeteranApplication.Data;

namespace VeteranApplication.Controllers
{
    public class VeteranController : Controller
    {
        private static List<Veteran> veterans = MockVeteranRepository.GetAllVeterans(); // Simulating database access

        // Show the Veteran database
        public IActionResult Index()
        {
            // Retrieve all veterans from the mock repository
            var veterans = MockVeteranRepository.GetAllVeterans();
            return View(veterans); // Pass the list to the view
        }

        // Filter Veterans
        [HttpPost]
        public IActionResult Filter(string gender, DateTime? startDate, DateTime? endDate)
        {
            var filteredVeterans = veterans.AsQueryable();

            if (!string.IsNullOrEmpty(gender))
                filteredVeterans = filteredVeterans.Where(v => v.Gender == gender);

            if (startDate.HasValue)
                filteredVeterans = filteredVeterans.Where(v => v.DateOfEntry >= startDate.Value);

            if (endDate.HasValue)
                filteredVeterans = filteredVeterans.Where(v => v.DateOfEntry <= endDate.Value);

            return View("Index", filteredVeterans.ToList());
        }
    }
}
