﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Extensions.Logging;
using VeteranAnalyticsSystem.Models;
using VeteranApplication.Data;
using VeteranAnalyticsSystem.Data;

namespace VeteranAnalyticsSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        // Static lists for veterans and events to simulate database access
        private static List<Veteran> veterans = MockVeteranRepository.GetAllVeterans();
        private static List<Event> events = MockEventRepository.GetAllEvents();
        private static List<Volunteer> volunteers = MockVolunteerRepository.GetAllVolunteers();

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        // Main landing page (Index)
        public IActionResult Index()
        {
            return View();
        }

        // Veterans view for listing/searching/filtering Veterans
        public IActionResult Veterans()
        {
            // Pass the full list of veterans to the view
            return View(veterans);
        }

        // Filter Veterans by gender and date range
        [HttpPost]
        public IActionResult FilterVeterans(string gender, DateTime? startDate, DateTime? endDate)
        {
            var filteredVeterans = veterans.AsQueryable();

            if (!string.IsNullOrEmpty(gender))
                filteredVeterans = filteredVeterans.Where(v => v.Gender == gender);

            if (startDate.HasValue)
                filteredVeterans = filteredVeterans.Where(v => v.DateOfEntry >= startDate.Value);

            if (endDate.HasValue)
                filteredVeterans = filteredVeterans.Where(v => v.DateOfEntry <= endDate.Value);

            return View("Veterans", filteredVeterans.ToList());
        }

        // Events view for listing/searching/filtering Events
        public IActionResult Events()
        {
            // Pass the full list of events to the view
            return View(events);
        }

        // Search Events by name
        [HttpPost]
        public IActionResult SearchEvents(string eventName)
        {
            var filteredEvents = events
                .Where(e => e.EventName.Contains(eventName, StringComparison.OrdinalIgnoreCase))
                .ToList();

            return View("Events", filteredEvents);
        }

        // Event Info view for detailed event information, showing participants by date
        public IActionResult EventInfo(int id)
        {
            var eventDetail = events.FirstOrDefault(e => e.EventId == id);
            return View(eventDetail);
        }

        // Privacy policy view
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        // Volunteers view to display all volunteers
        public IActionResult Volunteers()
        {
            return View(volunteers);
        }

        // Create a new volunteer - GET
        public IActionResult CreateVolunteer()
        {
            return View();
        }

        // Create a new volunteer - POST
        [HttpPost]
        public IActionResult CreateVolunteer(Volunteer volunteer)
        {
            if (ModelState.IsValid)
            {
                MockVolunteerRepository.AddVolunteer(volunteer);
                return RedirectToAction("Volunteers");
            }
            return View(volunteer);
        }

        // Edit an existing volunteer - GET
        public IActionResult EditVolunteer(int id)
        {
            var volunteer = MockVolunteerRepository.GetVolunteerById(id);
            if (volunteer == null)
            {
                return NotFound();
            }
            return View(volunteer);
        }

        // Edit an existing volunteer - POST
        [HttpPost]
        public IActionResult EditVolunteer(Volunteer volunteer)
        {
            if (ModelState.IsValid)
            {
                MockVolunteerRepository.UpdateVolunteer(volunteer);
                return RedirectToAction("Volunteers");
            }
            return View(volunteer);
        }

        // Confirm deletion of a volunteer - GET
        public IActionResult DeleteVolunteer(int id)
        {
            var volunteer = MockVolunteerRepository.GetVolunteerById(id);
            if (volunteer == null)
            {
                return NotFound();
            }
            return View(volunteer);
        }

        // Delete a volunteer - POST
        [HttpPost, ActionName("DeleteVolunteer")]
        public IActionResult DeleteConfirmed(int id)
        {
            MockVolunteerRepository.DeleteVolunteer(id);
            return RedirectToAction("Volunteers");
        }
    }
}
