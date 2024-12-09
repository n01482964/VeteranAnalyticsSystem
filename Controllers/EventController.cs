using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using VeteranAnalyticsSystem.Data;

namespace VeteranAnalyticsSystem.Controllers
{
    public class EventsController : Controller
    {
        private readonly GratitudeAmericaDbContext _context;

        public EventsController(GratitudeAmericaDbContext context)
        {
            _context = context;
        }

        // Display all events
        public async Task<IActionResult> Index()
        {
            var events = await _context.Events.ToListAsync();
            return View(events);
        }

        // Search events by name
        [HttpPost]
        public async Task<IActionResult> SearchEvents(string eventName)
        {
            var filteredEvents = await _context.Events
                .Where(e => e.EventName.Contains(eventName))
                .ToListAsync();

            return View("Index", filteredEvents);
        }

        // Show details for a specific event, including participants
        public async Task<IActionResult> Details(int id)
        {
            var eventDetail = await _context.Events
                .Include(e => e.Participants)
                .FirstOrDefaultAsync(e => e.EventId == id);

            if (eventDetail == null)
            {
                return NotFound();
            }

            return View(eventDetail);
        }
    }
}
