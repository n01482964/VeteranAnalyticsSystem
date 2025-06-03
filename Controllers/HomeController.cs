using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using VeteranAnalyticsSystem.Data;
using System;
using System.Collections.Generic;

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
            var veterans = await _context.Veterans.Include(v => v.Event).ToListAsync();
            var events = await _context.Events.ToListAsync();

            ViewBag.TotalVeterans = veterans.Count;
            ViewBag.TotalEvents = events.Count;

            // Gender Distribution (excluding Unknown)
            var genderLabels = veterans
                .Where(v => !string.IsNullOrEmpty(v.Gender) && v.Gender != "Unknown")
                .Select(v => v.Gender)
                .Distinct()
                .ToList();

            var genderData = genderLabels
                .Select(label => veterans.Count(v => v.Gender == label))
                .ToList();

            ViewBag.GenderLabels = genderLabels;
            ViewBag.GenderData = genderData;

            // Branch Distribution - Normalize and filter
            var validBranches = new[] { "Army", "National Guard", "Air Force", "Navy", "Marines", "Coast Guard" };
            var normalizedBranches = new List<string>();

            foreach (var vet in veterans)
            {
                if (!string.IsNullOrEmpty(vet.BranchOfService))
                {
                    var branches = vet.BranchOfService.Split(',').Select(b => b.Trim()).ToList();
                    var updatedBranches = new List<string>();

                    foreach (var branch in branches)
                    {
                        if (validBranches.Contains(branch))
                        {
                            updatedBranches.Add(branch);
                        }
                        else
                        {
                            updatedBranches.Add("Unknown");
                        }
                    }

                    vet.BranchOfService = string.Join(",", updatedBranches.Distinct());
                }
            }

            var filteredVeterans = veterans
                .Where(v => v.BranchOfService != null && !v.BranchOfService.Contains("Unknown"))
                .ToList();

            var allBranches = validBranches.ToList(); // Only valid branches for labels
            var branchData = allBranches
                .Select(branch => filteredVeterans.Count(v => v.BranchOfService.Split(',').Contains(branch)))
                .ToList();

            ViewBag.BranchLabels = allBranches;
            ViewBag.BranchData = branchData;

            // Age Range Distribution
            var ageRanges = new[] { "Under 25", "25-34", "35-44", "45-54", "55-64", "65+" };
            var ageRangeData = new int[ageRanges.Length];

            foreach (var vet in veterans)
            {
                if (vet.DateOfBirth != DateTime.MinValue)
                {
                    var age = DateTime.Now.Year - vet.DateOfBirth.Year;
                    if (vet.DateOfBirth.Date > DateTime.Now.AddYears(-age)) age--;

                    if (age < 25) ageRangeData[0]++;
                    else if (age < 35) ageRangeData[1]++;
                    else if (age < 45) ageRangeData[2]++;
                    else if (age < 55) ageRangeData[3]++;
                    else if (age < 65) ageRangeData[4]++;
                    else ageRangeData[5]++;
                }
            }

            ViewBag.AgeRangeLabels = ageRanges;
            ViewBag.AgeRangeData = ageRangeData;

            return View();
        }
    }
}
