using System;
using System.Collections.Generic;
using System.Linq;
using VeteranAnalyticsSystem.Models;

namespace VeteranAnalyticsSystem.Data
{
    public static class MockSyncStatusRepository
    {
        // Static list to hold sync status information
        private static List<SyncStatus> syncStatuses = new List<SyncStatus>
        {
            new SyncStatus { SyncId = 1, Source = "Ragic", Target = "SQL Database", LastSyncTime = DateTime.Now.AddHours(-2), Status = "Success", Details = "All data synced successfully." },
            new SyncStatus { SyncId = 2, Source = "Google Forms", Target = "SQL Database", LastSyncTime = DateTime.Now.AddHours(-4), Status = "Success", Details = "API Timeout encountered." },
            new SyncStatus { SyncId = 3, Source = "Ragic", Target = "SQL Database", LastSyncTime = DateTime.Now.AddDays(-1), Status = "Success", Details = "Sync completed with minor warnings." },
            new SyncStatus { SyncId = 4, Source = "Google Forms", Target = "SQL Database", LastSyncTime = DateTime.Now.AddDays(-2), Status = "Success", Details = "All data synced successfully." }
        };

        // Retrieve all sync statuses
        public static List<SyncStatus> GetAllSyncStatuses()
        {
            return syncStatuses;
        }

        // Retrieve a specific sync status by ID
        public static SyncStatus GetSyncStatusById(int id)
        {
            return syncStatuses.FirstOrDefault(s => s.SyncId == id);
        }
    }
}
