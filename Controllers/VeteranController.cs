using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using VeteranAnalyticsSystem.Data;

namespace VeteranAnalyticsSystem.Controllers
{
    public class VeteransController : Controller
    {
        private readonly GratitudeAmericaDbContext _context;

        public VeteransController(GratitudeAmericaDbContext context)
        {
            _context = context;
        }

        // Fetch all veterans and pass them to the view
        public IActionResult Index()
        {
            var veterans = _context.Veterans.ToList();
            return View(veterans);
        }

        // Filter veterans based on gender and enrollment dates
        [HttpPost]
        public IActionResult FilterVeterans(string gender, DateTime? startDate, DateTime? endDate)
        {
            var filteredVeterans = _context.Veterans.AsQueryable();

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
