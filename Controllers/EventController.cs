// Controllers/EventController.cs
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using VeteranAnalyticsSystem.Models;
using VeteranApplication.Data;

namespace VeteranApplication.Controllers
{
    public class EventController : Controller
    {
        private static List<Event> events = MockEventRepository.GetAllEvents(); // Simulating database access

        // Show the Event database
        public IActionResult Index()
        {
            // Get all events from the repository
            var events = MockEventRepository.GetAllEvents();
            return View(events); // Pass events list to the view
        }

        // Search Event by Name and View Participants by Date
        [HttpPost]
        public IActionResult Search(string eventName)
        {
            var filteredEvents = events.Where(e => e.EventName.Contains(eventName, StringComparison.OrdinalIgnoreCase)).ToList();
            return View("Index", filteredEvents);
        }

        // Show Event Details and Participants by Date
        public IActionResult Details(int id)
        {
            var eventDetail = events.FirstOrDefault(e => e.EventId == id);
            return View(eventDetail);
        }
    }
}
