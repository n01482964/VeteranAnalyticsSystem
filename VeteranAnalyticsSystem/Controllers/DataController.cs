using Microsoft.AspNetCore.Mvc;
using VeteranAnalyticsSystem.Data;
using VeteranAnalyticsSystem.Services;
using Microsoft.EntityFrameworkCore;
using VeteranAnalyticsSystem.Contracts;

namespace VeteranAnalyticsSystem.Controllers;

public class DataController(
    GratitudeAmericaDbContext context,
    RagicImporterService ragicImporterService,
    IGoogleFormsImporterService googleFormsImporterService) : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        ViewBag.TotalVeterans = context.Veterans.Count();
        ViewBag.TotalEvents = context.Events.Count();
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> DeleteAllSurveys()
    {
        try
        {
            var surveys = await context.Surveys.ToListAsync();
            context.Surveys.RemoveRange(surveys);
            await context.SaveChangesAsync();

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
            var veterans = await context.Veterans.ToListAsync();
            context.Veterans.RemoveRange(veterans);
            await context.SaveChangesAsync();

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
            var events = await context.Events.ToListAsync();
            context.Events.RemoveRange(events);
            await context.SaveChangesAsync();

            return Json(new { success = true, message = "All events deleted successfully." });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = $"Error deleting events: {ex.Message}" });
        }
    }

    [HttpPost]
    public async Task<IActionResult> SyncRagicDatabase()
    {
        try
        {
            var importedVeterans = await ragicImporterService.SyncVeteransFromRagicAsync();

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

    [HttpPost]
    public async Task<IActionResult> ImportFromGoogleForms()
    {
        await googleFormsImporterService.ImportForms();
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> ClearAzureDatabase()
    {
        try
        {
            var veterans = await context.Veterans.ToListAsync();
            var events = await context.Events.ToListAsync();
            var surveys = await context.Surveys.ToListAsync();

            context.Veterans.RemoveRange(veterans);
            context.Events.RemoveRange(events);
            context.Surveys.RemoveRange(surveys);

            await context.SaveChangesAsync();

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