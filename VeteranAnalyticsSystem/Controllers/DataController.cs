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
        private readonly RagicImporterService _ragicImporterService;
        //private readonly GoogleFormsImporterService _googleFormsImporterService;
        private readonly GratitudeAmericaDbContext _context;
        private readonly FileImporterService _fileImporterService;
        private readonly IConfiguration _configuration;

        public DataController(GratitudeAmericaDbContext context, IConfiguration configuration,
        RagicImporterService ragicImporterService)
        {
            _context = context;
            _fileImporterService = new FileImporterService(context);
            _ragicImporterService = ragicImporterService;
            //_googleFormsImporterService = googleFormsImporterService;

            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult Index() // ✅ Changed from DataImport()
        {
            ViewBag.TotalVeterans = _context.Veterans.Count();
            ViewBag.TotalEvents = _context.Events.Count();
            return View(); // ✅ Will now look for Views/Data/Index.cshtml
        }

        //[HttpPost]
        //public async Task<IActionResult> Import(string fileType)
        //{
        //    var service = new RagicImporterService(_configuration);
        //    await service.ImportVeteransBatchAsync();
        //    if (uploadfile == null || uploadfile.length == 0)
        //    {
        //        return json(new { success = false, message = "please select a file to upload." });
        //    }

        //    var uploadsfolder = path.combine(directory.getcurrentdirectory(), "wwwroot", "uploads");
        //    directory.createdirectory(uploadsfolder);

        //    var filepath = path.combine(uploadsfolder, path.getfilename(uploadfile.filename));
        //    using (var stream = new filestream(filepath, filemode.create))
        //    {
        //        await uploadfile.copytoasync(stream);
        //    }

        //    try
        //    {
        //        if (fileType == "VeteranData")
        //        {
        //            var veterans = _fileImporterService.ImportVeteransFromExcel("filePath");

        //            foreach (var vet in veterans)
        //            {
        //                SanitizeVeteranFields(vet);

        //                var existingVet = _context.Veterans.FirstOrDefault(v =>
        //                    v.Email.ToLower() == vet.Email.ToLower() &&
        //                    v.FirstName.ToLower() == vet.FirstName.ToLower() &&
        //                    v.LastName.ToLower() == vet.LastName.ToLower());

        //                if (existingVet != null) continue;

        //                _context.Veterans.Add(vet);
        //            }

        //            await _context.SaveChangesAsync();
        //        }
        //        else if (fileType == "PreRetreatSurvey" || fileType == "PostRetreatSurvey")
        //        {
        //            var surveyType = fileType == "PreRetreatSurvey" ? SurveyType.PreRetreat : SurveyType.PostRetreat;
        //            var surveys = _fileImporterService.ImportSurveys("filePath", surveyType);

        //            foreach (var survey in surveys)
        //            {
        //                _context.Surveys.Add(survey);
        //            }

        //            await _context.SaveChangesAsync();
        //        }
        //        else
        //        {
        //            return Json(new { success = false, message = "Unsupported file type." });
        //        }

        //        System.IO.File.Delete("filePath");
        //        return Json(new { success = true, message = "Import successful!" });
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { success = false, message = $"Error importing data: {ex.Message}" });
        //    }
        //}

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

        [HttpPost]
        public async Task<IActionResult> SyncRagicDatabase()
        {
            try
            {
                var importedVeterans = await _ragicImporterService.SyncVeteransFromRagicAsync();

                TempData["LastRagicSyncTime"] = DateTime.Now;
                return Json(new
                {
                    success = true,
                    message = $"Ragic sync complete. {importedVeterans.Count} new veterans imported.",
                    lastSync = TempData["LastRagicSyncTime"]
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = $"Ragic sync failed: {ex.Message}"
                });
            }
        }



        //[HttpPost]
        //public IActionResult SyncGoogleFormsDatabase()
        //{
        //    TempData["LastGoogleSyncTime"] = DateTime.Now.ToString("f");
        //    return Json(new { success = true, message = "Google Forms Database synced successfully.", lastSync = TempData["LastGoogleSyncTime"] });
        //}

        //[HttpGet]
        //public IActionResult GetLastSyncTimes()
        //{
        //    var ragicSync = TempData["LastRagicSyncTime"] ?? "Never";
        //    var googleSync = TempData["LastGoogleSyncTime"] ?? "Never";

        //    return Json(new { ragic = ragicSync, google = googleSync });
        //}

        [HttpPost]
        public async Task<IActionResult> ClearAzureDatabase()
        {
            try
            {
                var veterans = await _context.Veterans.ToListAsync();
                var events = await _context.Events.ToListAsync();
                var surveys = await _context.Surveys.ToListAsync();

                _context.Veterans.RemoveRange(veterans);
                _context.Events.RemoveRange(events);
                _context.Surveys.RemoveRange(surveys);

                await _context.SaveChangesAsync();

                var timestamp = DateTime.Now.ToString("f");
                return Json(new
                {
                    success = true,
                    message = "Azure database cleared successfully.",
                    lastCleared = timestamp
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = $"Error clearing Azure database: {ex.Message}"
                });
            }
        }

    }
}
