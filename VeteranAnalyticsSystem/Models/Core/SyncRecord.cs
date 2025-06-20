using VeteranAnalyticsSystem.Models.Enums;

namespace VeteranAnalyticsSystem.Models.Core;

public class SyncRecord
{
    public int Id { get; set; }

    public DateTime TimeStamp { get; set; }

    public SyncTypes SyncType { get; set; }
}
