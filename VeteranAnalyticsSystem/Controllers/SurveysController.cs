using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using VeteranAnalyticsSystem.Data;
using VeteranAnalyticsSystem.Models;

namespace VeteranAnalyticsSystem.Controllers
{
    public class SurveysController : Controller
    {
        private readonly GratitudeAmericaDbContext _context;

        public SurveysController(GratitudeAmericaDbContext context)
        {
            _context = context;
        }

        // GET: Surveys/Index
        public async Task<IActionResult> Index()
        {
            // Retrieve all surveys from the database.
            var surveys = await _context.Surveys.ToListAsync();
            return View(surveys);
        }

        // GET: Surveys/Details/{id}
        public async Task<IActionResult> Details(int id)
        {
            // Retrieve a single survey based on its SurveyId.
            var survey = await _context.Surveys.FirstOrDefaultAsync(s => s.SurveyId == id);
            if (survey == null)
            {
                return NotFound();
            }
            return View(survey);
        }
    }
}
