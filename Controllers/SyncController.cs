using Microsoft.AspNetCore.Mvc;
using VeteranAnalyticsSystem.Data;
using VeteranAnalyticsSystem.Models;

namespace VeteranAnalyticsSystem.Controllers
{
    public class SyncController : Controller
    {
        // Display the Settings view with sync status
        public IActionResult Settings(int? id)
        {
            if (id == null)
            {
                // If no ID is provided, show all sync statuses
                var syncStatuses = MockSyncStatusRepository.GetAllSyncStatuses();
                return View("Settings", syncStatuses); // Points to Views/Home/Settings.cshtml
            }
            else
            {
                // If ID is provided, show details for a specific sync
                var syncStatus = MockSyncStatusRepository.GetSyncStatusById(id.Value);
                if (syncStatus == null)
                {
                    return NotFound(); // Return a 404 if the sync status does not exist
                }
                return View("Details", syncStatus); // Use a separate view for details
            }
        }
    }
}
