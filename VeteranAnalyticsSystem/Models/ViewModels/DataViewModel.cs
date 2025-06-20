namespace VeteranAnalyticsSystem.Models.ViewModels;

public class DataViewModel
{
    public int TotalVeterans { get; set; }

    public int TotalEvents { get; set; }

    public DateTime? LastRagicSync { get; set; }

    public DateTime? LastGoogleFormsSync { get; set; }

    public bool Error { get; set; }

    public string? Message { get; set; }

    public string DisplayLastGoogleFormsSync => LastGoogleFormsSync.HasValue ? LastGoogleFormsSync.Value.ToShortDateString() : "Never";
}
