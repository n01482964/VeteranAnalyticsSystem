using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using VeteranAnalyticsSystem.Data;
using VeteranAnalyticsSystem.Models;

namespace VeteranAnalyticsSystem.Controllers
{
    public class VeteransController : Controller
    {
        private readonly GratitudeAmericaDbContext _context;

        public VeteransController(GratitudeAmericaDbContext context)
        {
            _context = context;
        }

        // GET: Veterans/Index
        public IActionResult Index()
        {
            var veterans = _context.Veterans.ToList();
            return View(veterans);
        }

        // POST: Veterans/FilterVeterans
        [HttpPost]
        public IActionResult FilterVeterans(string gender, DateTime? startDate, DateTime? endDate)
        {
            var filteredVeterans = _context.Veterans.AsQueryable();

            if (!string.IsNullOrEmpty(gender))
            {
                filteredVeterans = filteredVeterans.Where(v => v.Gender == gender);
            }

            // When both startDate and endDate are provided,
            // select veterans whose service period overlaps the search interval.
            if (startDate.HasValue && endDate.HasValue)
            {
                filteredVeterans = filteredVeterans.Where(v =>
                    v.StartOfService <= endDate.Value &&
                    v.EndOfService >= startDate.Value);
            }
            // If only startDate is provided, return veterans whose service ended on or after the search start.
            else if (startDate.HasValue)
            {
                filteredVeterans = filteredVeterans.Where(v =>
                    v.EndOfService >= startDate.Value);
            }
            // If only endDate is provided, return veterans who started service on or before the search end.
            else if (endDate.HasValue)
            {
                filteredVeterans = filteredVeterans.Where(v =>
                    v.StartOfService <= endDate.Value);
            }

            return View("Index", filteredVeterans.ToList());
        }

        // GET: Veterans/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Veterans/Create
        [HttpPost]
        public async Task<IActionResult> Create(Veteran veteran)
        {
            if (ModelState.IsValid)
            {
                // Look up an existing event using the veteran's RetreatDateLocation.
                var existingEvent = _context.Events
                    .FirstOrDefault(e => e.EventName == veteran.RetreatDateLocation);

                if (existingEvent == null)
                {
                    // Create a new event if one does not exist.
                    var newEvent = new Event
                    {
                        EventName = veteran.RetreatDateLocation,
                        // Optionally, parse out the actual location and event date from the string.
                        Location = veteran.RetreatDateLocation,
                        EventDate = DateTime.Now // Replace with the actual event date if available.
                    };
                    _context.Events.Add(newEvent);
                    await _context.SaveChangesAsync();
                    existingEvent = newEvent;
                }

                // Check if the veteran already exists (using the unique VeteranId generated from email).
                var existingVeteran = _context.Veterans.FirstOrDefault(v => v.VeteranId == veteran.VeteranId);

                if (existingVeteran != null)
                {
                    // If the veteran already exists, add the event if it is not already linked.
                    if (!existingVeteran.Events.Any(e => e.EventId == existingEvent.EventId))
                    {
                        existingVeteran.Events.Add(existingEvent);
                    }
                    _context.Veterans.Update(existingVeteran);
                }
                else
                {
                    // New veteran: add the event to the veteran's event list.
                    veteran.Events.Add(existingEvent);
                    _context.Veterans.Add(veteran);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(veteran);
        }
    }
}
