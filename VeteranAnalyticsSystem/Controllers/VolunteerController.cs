using Microsoft.AspNetCore.Mvc;
using System.Linq;
using VeteranAnalyticsSystem.Data;
using VeteranAnalyticsSystem.Models.Core;

namespace VeteranAnalyticsSystem.Controllers
{
    public class VolunteersController : Controller
    {
        private readonly GratitudeAmericaDbContext _context;

        public VolunteersController(GratitudeAmericaDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var volunteers = _context.Volunteers.ToList();
            return View(volunteers);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Volunteer volunteer)
        {
            if (ModelState.IsValid)
            {
                _context.Volunteers.Add(volunteer);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(volunteer);
        }

        public IActionResult Edit(int id)
        {
            var volunteer = _context.Volunteers.FirstOrDefault(v => v.VolunteerId == id);
            if (volunteer == null)
            {
                return NotFound();
            }
            return View(volunteer);
        }

        [HttpPost]
        public IActionResult Edit(Volunteer volunteer)
        {
            if (ModelState.IsValid)
            {
                _context.Volunteers.Update(volunteer);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(volunteer);
        }

        public IActionResult Delete(int id)
        {
            var volunteer = _context.Volunteers.FirstOrDefault(v => v.VolunteerId == id);
            if (volunteer == null)
            {
                return NotFound();
            }
            return View(volunteer);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var volunteer = _context.Volunteers.FirstOrDefault(v => v.VolunteerId == id);
            if (volunteer != null)
            {
                _context.Volunteers.Remove(volunteer);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}
