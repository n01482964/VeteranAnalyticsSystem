namespace VeteranAnalyticsSystem.Models.ViewModels;

public class DataViewModel
{
    public int TotalVeterans { get; set; }

    public int TotalEvents { get; set; }

    public DateTime? LastRagicSync { get; set; }

    public DateTime? LastGoogleFormsSync { get; set; }

    public string DisplayLastGoogleFormsSync => LastGoogleFormsSync.HasValue ? LastGoogleFormsSync.Value.ToShortDateString() : "Never";
}
