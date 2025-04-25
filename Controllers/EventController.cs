using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using VeteranAnalyticsSystem.Data;
using VeteranAnalyticsSystem.Models;
using System.Collections.Generic;

namespace VeteranAnalyticsSystem.Controllers
{
    public class EventsController : Controller
    {
        private readonly GratitudeAmericaDbContext _context;

        public EventsController(GratitudeAmericaDbContext context)
        {
            _context = context;
        }

        // Optimized Index: only load minimal data + participant counts
        public async Task<IActionResult> Index()
        {
            var events = await _context.Events
                .Select(e => new Event
                {
                    EventId = e.EventId,
                    Location = e.Location,
                    EventDate = e.EventDate,
                    Veterans = new List<Veteran>() // placeholder, we’ll count separately
                })
                .ToListAsync();

            // Get participant counts efficiently
            var eventIds = events.Select(e => e.EventId).ToList();
            var counts = await _context.Veterans
                .Where(v => eventIds.Contains(v.EventId))
                .GroupBy(v => v.EventId)
                .Select(g => new { EventId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.EventId, x => x.Count);

            foreach (var evt in events)
            {
                evt.Veterans = new List<Veteran>(new Veteran[counts.GetValueOrDefault(evt.EventId)]);
            }

            return View(events);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var evt = await _context.Events
                .Include(e => e.Veterans)
                .FirstOrDefaultAsync(e => e.EventId == id);

            if (evt == null) return NotFound();

            var eventStart = evt.EventDate;
            var veteranEmails = evt.Veterans.Select(v => v.Email).ToList();
            var postRetreatDate = eventStart.AddDays(2);

            var surveys = await _context.Surveys
                .Where(s => veteranEmails.Contains(s.Email)
                    && s.SurveyType == SurveyType.PostRetreat
                    && s.SubmissionDate.Date == postRetreatDate.Date)
                .ToListAsync();

            var viewModel = new EventDetailsViewModel
            {
                Event = evt,
                Veterans = evt.Veterans.ToList(),
                MatchedSurveys = surveys
            };

            return View(viewModel);
        }


        // Summary route (could be used for direct links)
        public async Task<IActionResult> EventSummary()
        {
            var events = await _context.Events
                .Include(e => e.Veterans)
                .ToListAsync();

            var grouped = GroupEventsByLocation(events);
            return View("Index", grouped);
        }

        // ✅ Reusable private method to group events by location
        private Dictionary<string, List<Event>> GroupEventsByLocation(List<Event> events)
        {
            return events
                .GroupBy(e => e.Location)
                .ToDictionary(g => g.Key, g => g.OrderBy(e => e.EventDate).ToList());
        }
    }
}
