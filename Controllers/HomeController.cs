using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using VeteranAnalyticsSystem.Data;

namespace VeteranAnalyticsSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly GratitudeAmericaDbContext _context;

        public HomeController(GratitudeAmericaDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Load veterans including their event to avoid null reference or column issues
            var veterans = await _context.Veterans
                .Include(v => v.Event)
                .ToListAsync();

            // Totals
            ViewBag.TotalVeterans = veterans.Count;
            ViewBag.TotalEvents = await _context.Events.CountAsync();

            // Gender chart
            var genderLabels = veterans
                .Where(v => !string.IsNullOrEmpty(v.Gender))
                .Select(v => v.Gender)
                .Distinct()
                .ToList();

            var genderData = genderLabels
                .Select(label => veterans.Count(v => v.Gender == label))
                .ToList();

            ViewBag.GenderLabels = genderLabels;
            ViewBag.GenderData = genderData;

            // Branch chart
            var allBranches = veterans
                .Where(v => !string.IsNullOrEmpty(v.BranchOfService))
                .SelectMany(v => v.BranchOfService.Split(',').Select(b => b.Trim()))
                .Distinct()
                .ToList();

            var branchData = allBranches
                .Select(branch =>
                    veterans.Count(v => v.BranchOfService.Split(',').Select(b => b.Trim()).Contains(branch)))
                .ToList();

            ViewBag.BranchLabels = allBranches;
            ViewBag.BranchData = branchData;

            return View(veterans);
        }
    }
}
