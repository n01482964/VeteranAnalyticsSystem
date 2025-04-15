using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
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

        public IActionResult Index()
        {
            var veterans = _context.Veterans
                .Include(v => v.Event)
                .ToList();

            var distinctBranches = veterans
                .Select(v => v.BranchOfService)
                .SelectMany(b => b.Split(',').Select(b => b.Trim()))
                .Distinct()
                .ToList();

            ViewBag.Branches = distinctBranches;
            ViewBag.EventLocations = _context.Events.Select(e => e.Location).Distinct().ToList();
            ViewBag.Dates = _context.Events.Select(e => e.EventDate).Distinct().ToList();

            var veteranLabels = veterans.Select(v => v.Gender).Distinct().ToList();
            var veteranData = veterans.GroupBy(v => v.Gender).Select(g => g.Count()).ToList();
            var eventLabels = new[] { "Army", "Navy", "Air Force", "Marines" };
            var eventData = new[] { 50, 30, 20, 15 };

            ViewBag.VeteranLabels = veteranLabels;
            ViewBag.VeteranData = veteranData;
            ViewBag.EventLabels = eventLabels;
            ViewBag.EventData = eventData;

            return View(veterans);
        }

        [HttpPost]
        public IActionResult FilterVeterans(string gender, string[] branches, DateTime? startDate, DateTime? endDate, string eventLocation, DateTime? eventDate)
        {
            var filteredVeterans = _context.Veterans
                .Include(v => v.Event)
                .AsQueryable();

            if (!string.IsNullOrEmpty(gender))
                filteredVeterans = filteredVeterans.Where(v => v.Gender == gender);

            if (branches != null && branches.Any())
                filteredVeterans = filteredVeterans.Where(v => branches.Any(branch => v.BranchOfService.Contains(branch)));

            if (startDate.HasValue && endDate.HasValue)
                filteredVeterans = filteredVeterans.Where(v => v.StartOfService <= endDate.Value && v.EndOfService >= startDate.Value);
            else if (startDate.HasValue)
                filteredVeterans = filteredVeterans.Where(v => v.EndOfService >= startDate.Value);
            else if (endDate.HasValue)
                filteredVeterans = filteredVeterans.Where(v => v.StartOfService <= endDate.Value);

            if (!string.IsNullOrWhiteSpace(eventLocation))
                filteredVeterans = filteredVeterans.Where(v => v.Event != null && v.Event.Location == eventLocation);

            if (eventDate.HasValue)
                filteredVeterans = filteredVeterans.Where(v => v.Event != null && v.Event.EventDate.Date == eventDate.Value.Date);

            return View("Index", filteredVeterans.ToList());
        }

        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(Veteran veteran)
        {
            if (ModelState.IsValid)
            {
                var existingEvent = _context.Events.FirstOrDefault(e =>
                    e.Location == veteran.City && e.EventDate.Date == veteran.StartOfService.Date);

                if (existingEvent == null)
                {
                    existingEvent = new Event
                    {
                        Location = veteran.City,
                        EventDate = veteran.StartOfService
                    };
                    _context.Events.Add(existingEvent);
                    await _context.SaveChangesAsync();
                }

                veteran.EventId = existingEvent.EventId;
                _context.Veterans.Add(veteran);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(veteran);
        }

        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();
            var veteran = await _context.Veterans.FindAsync(id);
            return veteran == null ? NotFound() : View(veteran);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(string id, Veteran updatedVeteran)
        {
            if (id != updatedVeteran.VeteranId)
                return NotFound();

            if (!ModelState.IsValid)
                return View(updatedVeteran);

            try
            {
                _context.Update(updatedVeteran);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Veterans.Any(e => e.VeteranId == id))
                    return NotFound();
                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(string id)
        {
            var veteran = await _context.Veterans
                .Include(v => v.Event)
                .FirstOrDefaultAsync(v => v.VeteranId == id);

            return veteran == null ? NotFound() : View(veteran);
        }

        // POST: Veterans/Rate
        [HttpPost]
        public async Task<IActionResult> Rate(string id, int? newRating)
        {
            if (string.IsNullOrEmpty(id) || newRating is < 1 or > 5)
            {
                return RedirectToAction(nameof(Details), new { id });
            }

            var veteran = await _context.Veterans.FindAsync(id);
            if (veteran == null)
                return NotFound();

            veteran.StarRating = newRating;
            _context.Veterans.Update(veteran);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id });
        }
    }
}
