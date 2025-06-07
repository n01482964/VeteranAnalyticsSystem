using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VeteranAnalyticsSystem.Data;
using VeteranAnalyticsSystem.Models;
using VeteranAnalyticsSystem.Services;
using CsvHelper;
using System.Globalization;
using System.Text;



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
        public IActionResult FilterVeterans(int[] starRating, string gender, string eventLocation, DateTime? startDate, DateTime? endDate)
        {
            var filteredVeterans = _context.Veterans
                .Include(v => v.Event)
                .AsQueryable();

            // Filter by gender
            if (!string.IsNullOrEmpty(gender))
                filteredVeterans = filteredVeterans.Where(v => v.Gender == gender);

            // Filter by star ratings
            if (starRating != null && starRating.Any())
            {
                var ratedStars = starRating.Where(r => r > 0).ToList();
                bool includeUnrated = starRating.Contains(0);

                if (includeUnrated && ratedStars.Any())
                {
                    // Include both rated and unrated
                    filteredVeterans = filteredVeterans.Where(v =>
                        (!v.StarRating.HasValue && includeUnrated) ||
                        (v.StarRating.HasValue && ratedStars.Contains(v.StarRating.Value)));
                }
                else if (includeUnrated)
                {
                    // Only unrated
                    filteredVeterans = filteredVeterans.Where(v => !v.StarRating.HasValue);
                }
                else
                {
                    // Only rated stars
                    filteredVeterans = filteredVeterans.Where(v => v.StarRating.HasValue && ratedStars.Contains(v.StarRating.Value));
                }
            }

            // Filter by event location
            if (!string.IsNullOrWhiteSpace(eventLocation))
                filteredVeterans = filteredVeterans.Where(v => v.Event != null && v.Event.Location == eventLocation);

            // Filter by service start and end dates
            if (startDate.HasValue && endDate.HasValue)
                filteredVeterans = filteredVeterans.Where(v => v.StartOfService <= endDate.Value && v.EndOfService >= startDate.Value);
            else if (startDate.HasValue)
                filteredVeterans = filteredVeterans.Where(v => v.EndOfService >= startDate.Value);
            else if (endDate.HasValue)
                filteredVeterans = filteredVeterans.Where(v => v.StartOfService <= endDate.Value);

            // Optional: Refresh ViewBag content
            ViewBag.EventLocations = _context.Events.Select(e => e.Location).Distinct().ToList();

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

        [HttpPost]
        public IActionResult ExportToCsv([FromBody] ExportRequest request)
        {
            if (request.VeteranIds == null || !request.VeteranIds.Any())
            {
                return BadRequest("No veterans selected for export.");
            }

            var veterans = _context.Veterans
                .Include(v => v.Event)
                .Where(v => request.VeteranIds.Contains(v.VeteranId))
                .ToList();

            using var memoryStream = new MemoryStream();
            using var writer = new StreamWriter(memoryStream, new UTF8Encoding(true)); // Adds BOM for accents
            using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

            // Write header
            csv.WriteField("First Name");
            csv.WriteField("Last Name");
            csv.WriteField("Email");
            csv.WriteField("Phone");
            csv.WriteField("Home Address");
            csv.WriteField("City");
            csv.WriteField("State");
            csv.WriteField("Relationship Status");
            csv.WriteField("Gender");
            csv.WriteField("DOB");
            csv.WriteField("Military Service Status");
            csv.WriteField("Highest Rank");
            csv.WriteField("Service Start");
            csv.WriteField("Service End");
            csv.WriteField("Branch");
            csv.WriteField("Number of Deployments");
            csv.WriteField("Deployment Details");
            csv.WriteField("Health Concerns");
            csv.WriteField("Additional Health Info");
            csv.WriteField("Physical Limitations");
            csv.WriteField("Retreat Location");
            csv.WriteField("Retreat Date");
            csv.WriteField("Star Rating");
            csv.NextRecord();

            foreach (var v in veterans)
            {
                csv.WriteField(v.FirstName);
                csv.WriteField(v.LastName);
                csv.WriteField(v.Email);
                csv.WriteField(v.PhoneNumber);
                csv.WriteField(v.HomeAddress);
                csv.WriteField(v.City);
                csv.WriteField(v.State);
                csv.WriteField(v.RelationshipStatus);
                csv.WriteField(v.Gender);
                csv.WriteField(v.DateOfBirth.ToShortDateString());
                csv.WriteField(v.MilitaryServiceStatus);
                csv.WriteField(v.HighestRank);
                csv.WriteField(v.StartOfService.ToShortDateString());
                csv.WriteField(v.EndOfService.ToShortDateString());
                csv.WriteField(v.BranchOfService);
                csv.WriteField(v.NumberOfDeployments);
                csv.WriteField(v.DeploymentDetails);
                csv.WriteField(v.HealthConcerns);
                csv.WriteField(v.AdditionalHealthInfo);
                csv.WriteField(v.PhysicalLimitations);
                csv.WriteField(v.Event?.Location);
                csv.WriteField(v.Event?.EventDate.ToShortDateString());
                csv.WriteField(v.StarRating.HasValue ? $"{v.StarRating} ★" : "Unrated");
                csv.NextRecord();
            }

            writer.Flush();
            var result = memoryStream.ToArray();
            
            // FINAL FIX: Only use what the user provided, no logic, no appending
            var cleanFileName = request.OutputFileName.Trim();
            if (!cleanFileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
            {
                cleanFileName += ".csv";
            }

            return File(result, "text/csv", cleanFileName);
        }


        public class ExportRequest
        {
            public List<string> VeteranIds { get; set; } = new List<string>();
            public string OutputFileName { get; set; } = string.Empty;
        }
    }
}
