using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using VeteranAnalyticsSystem.Data;
using VeteranAnalyticsSystem.Services;
using VeteranAnalyticsSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace VeteranAnalyticsSystem.Controllers
{
    public class DataController : Controller
    {
        private readonly GratitudeAmericaDbContext _context;
        private readonly FileImporterService _fileImporterService;

        public DataController(GratitudeAmericaDbContext context)
        {
            _context = context;
            _fileImporterService = new FileImporterService(context);
        }

        [HttpGet]
        public IActionResult Index() // ✅ Changed from DataImport()
        {
            ViewBag.TotalVeterans = _context.Veterans.Count();
            ViewBag.TotalEvents = _context.Events.Count();
            return View(); // ✅ Will now look for Views/Data/Index.cshtml
        }

        [HttpPost]
        public async Task<IActionResult> Import(IFormFile uploadFile, string fileType)
        {
            if (uploadFile == null || uploadFile.Length == 0)
            {
                return Json(new { success = false, message = "Please select a file to upload." });
            }

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            Directory.CreateDirectory(uploadsFolder);

            var filePath = Path.Combine(uploadsFolder, Path.GetFileName(uploadFile.FileName));
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await uploadFile.CopyToAsync(stream);
            }

            try
            {
                if (fileType == "VeteranData")
                {
                    var veterans = _fileImporterService.ImportVeteransFromExcel(filePath);

                    foreach (var vet in veterans)
                    {
                        SanitizeVeteranFields(vet);

                        var existingVet = _context.Veterans.FirstOrDefault(v =>
                            v.Email.ToLower() == vet.Email.ToLower() &&
                            v.FirstName.ToLower() == vet.FirstName.ToLower() &&
                            v.LastName.ToLower() == vet.LastName.ToLower());

                        if (existingVet != null) continue;

                        _context.Veterans.Add(vet);
                    }

                    await _context.SaveChangesAsync();
                }
                else if (fileType == "PreRetreatSurvey" || fileType == "PostRetreatSurvey")
                {
                    var surveyType = fileType == "PreRetreatSurvey" ? SurveyType.PreRetreat : SurveyType.PostRetreat;
                    var surveys = _fileImporterService.ImportSurveys(filePath, surveyType);

                    foreach (var survey in surveys)
                    {
                        _context.Surveys.Add(survey);
                    }

                    await _context.SaveChangesAsync();
                }
                else
                {
                    return Json(new { success = false, message = "Unsupported file type." });
                }

                System.IO.File.Delete(filePath);
                return Json(new { success = true, message = "Import successful!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error importing data: {ex.Message}" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAllSurveys()
        {
            try
            {
                var surveys = await _context.Surveys.ToListAsync();
                _context.Surveys.RemoveRange(surveys);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "All surveys deleted successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error deleting surveys: {ex.Message}" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAllVeterans()
        {
            try
            {
                var veterans = await _context.Veterans.ToListAsync();
                _context.Veterans.RemoveRange(veterans);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "All veterans deleted successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error deleting veterans: {ex.Message}" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAllEvents()
        {
            try
            {
                var events = await _context.Events.ToListAsync();
                _context.Events.RemoveRange(events);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "All events deleted successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error deleting events: {ex.Message}" });
            }
        }

        private void SanitizeVeteranFields(Veteran vet)
        {
            vet.FirstName = string.IsNullOrWhiteSpace(vet.FirstName) ? "N/A" : vet.FirstName;
            vet.LastName = string.IsNullOrWhiteSpace(vet.LastName) ? "N/A" : vet.LastName;
            vet.Email = string.IsNullOrWhiteSpace(vet.Email) ? $"unknown-{Guid.NewGuid()}@example.com" : vet.Email;
            vet.PhoneNumber = string.IsNullOrWhiteSpace(vet.PhoneNumber) ? "N/A" : vet.PhoneNumber;
            vet.HomeAddress = string.IsNullOrWhiteSpace(vet.HomeAddress) ? "Unknown" : vet.HomeAddress;
            vet.City = string.IsNullOrWhiteSpace(vet.City) ? "Unknown" : vet.City;
            vet.State = string.IsNullOrWhiteSpace(vet.State) ? "Unknown" : vet.State;
            vet.RelationshipStatus = string.IsNullOrWhiteSpace(vet.RelationshipStatus) ? "Unknown" : vet.RelationshipStatus;
            vet.Gender = string.IsNullOrWhiteSpace(vet.Gender) ? "Unknown" : vet.Gender;
            vet.MilitaryServiceStatus = string.IsNullOrWhiteSpace(vet.MilitaryServiceStatus) ? "Unknown" : vet.MilitaryServiceStatus;
            vet.HighestRank = string.IsNullOrWhiteSpace(vet.HighestRank) ? "Unknown" : vet.HighestRank;
            vet.BranchOfService = string.IsNullOrWhiteSpace(vet.BranchOfService) ? "Unknown" : vet.BranchOfService;
            vet.DeploymentDetails = string.IsNullOrWhiteSpace(vet.DeploymentDetails) ? "None" : vet.DeploymentDetails;
            vet.HealthConcerns = string.IsNullOrWhiteSpace(vet.HealthConcerns) ? "None" : vet.HealthConcerns;
            vet.AdditionalHealthInfo = string.IsNullOrWhiteSpace(vet.AdditionalHealthInfo) ? "None" : vet.AdditionalHealthInfo;
            vet.PhysicalLimitations = string.IsNullOrWhiteSpace(vet.PhysicalLimitations) ? "None" : vet.PhysicalLimitations;
        }
    }
}
