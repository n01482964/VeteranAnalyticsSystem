using Microsoft.AspNetCore.Mvc;
using VeteranAnalyticsSystem.Data;
using VeteranAnalyticsSystem.Services;
using Microsoft.EntityFrameworkCore;
using VeteranAnalyticsSystem.Contracts;
using VeteranAnalyticsSystem.Models.ViewModels;
using VeteranAnalyticsSystem.Models.Enums;

namespace VeteranAnalyticsSystem.Controllers;

public class DataController(
    GratitudeAmericaDbContext context,
    RagicImporterService ragicImporterService,
    IGoogleFormsImporterService googleFormsImporterService) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        return View(await GetIndexViewModel());
    }

    [HttpPost]
    public async Task<IActionResult> DeleteAllSurveys()
    {
        context.SyncRecords.RemoveRange(context.SyncRecords);
        context.Surveys.RemoveRange(context.Surveys);
        await context.SaveChangesAsync();

        var viewModel = await GetIndexViewModel();
        viewModel.Message = "All surveys deleted successfully.";

        return View("Index", viewModel);
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
        var count = await googleFormsImporterService.ImportForms();

        var viewModel = await GetIndexViewModel();
        viewModel.Message = count == 0 ? "No new items synced" : $"{count} new items synced";

        return View("Index", viewModel);
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

    private async Task<DataViewModel> GetIndexViewModel()
    {
        var lastGoogleFormsSync = await context.SyncRecords
                .Where(s => s.SyncType == SyncTypes.GoogleForms)
                .OrderByDescending(s => s.TimeStamp)
                .Select(s => s.TimeStamp)
                .FirstOrDefaultAsync();
        var lastRagicSync = await context.SyncRecords
                .Where(s => s.SyncType == SyncTypes.Ragic)
                .OrderByDescending(s => s.TimeStamp)
                .Select(s => s.TimeStamp)
                .FirstOrDefaultAsync();

        return new DataViewModel
        {
            LastGoogleFormsSync = lastGoogleFormsSync != default ? lastGoogleFormsSync : null,
            LastRagicSync = lastRagicSync != default ? lastGoogleFormsSync : null,
            TotalVeterans = context.Veterans.Count(),
            TotalEvents = context.Events.Count()
        };
    }
}