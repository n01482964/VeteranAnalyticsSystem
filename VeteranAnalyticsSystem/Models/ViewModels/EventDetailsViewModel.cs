using Microsoft.AspNetCore.Mvc.Rendering;
using VeteranAnalyticsSystem.Models.Core;

namespace VeteranAnalyticsSystem.Models.ViewModels;

public class EventDetailsViewModel
{
    public required Event Event { get; set; }

    public required List<Veteran> Veterans { get; set; }

    public required List<Survey> MatchedSurveys { get; set; }

    public List<SelectListItem> Ratings =
    [
        new() { Value = "", Text = "" },
        new() { Value = "1", Text = "1" },
        new() { Value = "2", Text = "2" },
        new() { Value = "3", Text = "3" },
        new() { Value = "4", Text = "4" },
        new() { Value = "5", Text = "5" }
    ];

}
