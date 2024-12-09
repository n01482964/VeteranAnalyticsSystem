using System;

namespace VeteranAnalyticsSystem.Models
{
    public class SyncStatus
    {
        public int SyncId { get; set; } // Unique ID for the sync operation
        public string Source { get; set; } // Data source (e.g., Ragic, Google Forms)
        public string Target { get; set; } // Data target (e.g., SQL Database)
        public DateTime LastSyncTime { get; set; } // Last time the sync was attempted
        public string Status { get; set; } // Status of the sync (e.g., Success, Failed)
        public string Details { get; set; } // Additional details about the sync

    }
}
