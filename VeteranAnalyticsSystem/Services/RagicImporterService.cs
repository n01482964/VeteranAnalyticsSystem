using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using VeteranAnalyticsSystem.Data;
using VeteranAnalyticsSystem.Models;

namespace VeteranAnalyticsSystem.Services
{
    public class RagicImporterService
    {
        private readonly GratitudeAmericaDbContext _context;
        private readonly HttpClient _httpClient;
        private readonly string _apiUrl;

        public RagicImporterService(GratitudeAmericaDbContext context, IConfiguration config)
        {
            _context = context;
            _httpClient = new HttpClient();

            var apiKey = config["RagicAPIKey"];
            _apiUrl = config.GetValue<string>("RagicAPIUrl");

            if (string.IsNullOrWhiteSpace(apiKey) || string.IsNullOrWhiteSpace(_apiUrl))
                throw new Exception("Ragic API configuration is missing.");

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", apiKey);
        }

        public async Task<List<Veteran>> SyncVeteransFromRagicAsync()
        {
            var response = await _httpClient.GetAsync($"{_apiUrl}?api");
            if (!response.IsSuccessStatusCode)
                throw new Exception("Failed to fetch Ragic data");

            var json = await response.Content.ReadAsStringAsync();
            var ragicRecordsDictionary = JsonSerializer.Deserialize<Dictionary<string, RagicVeteranDto>>(json);
            var ragicRecords = ragicRecordsDictionary.Select(x => x.Value).ToList();

            var importedVeterans = new List<Veteran>();

            //int count = 0;
            foreach (var record in ragicRecords)
            {
                //if (count >= 100) break;
                //count++;

                if (string.IsNullOrWhiteSpace(record.Email))
                    continue;

                string rawEvent = record.EventLocationDate ?? "";
                string location = "Unknown";
                DateTime eventDate = DateTime.Now;

                if (!string.IsNullOrWhiteSpace(rawEvent))
                {
                    // Expected format: "Lake Oconee, Georgia - April 25-28, 2019"
                    var parts = rawEvent.Split(" - ", StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length == 2)
                    {
                        location = NormalizeStateAndLocation(parts[0]);

                        // Parse the date range, e.g., "April 25-28, 2019"
                        var dateRange = parts[1];

                        // Match the start day and year from the range (assumes format like: "April 25-28, 2019")
                        var dateMatch = Regex.Match(dateRange, @"(?<month>\w+)\s+(?<day>\d{1,2})-\d{1,2},\s*(?<year>\d{4})");

                        if (dateMatch.Success)
                        {
                            string month = dateMatch.Groups["month"].Value;
                            string day = dateMatch.Groups["day"].Value;
                            string year = dateMatch.Groups["year"].Value;

                            if (DateTime.TryParse($"{month} {day}, {year}", out var parsedDate))
                            {
                                eventDate = parsedDate;
                            }
                        }
                    }
                }

                // Avoid duplicate veterans
                if (_context.Veterans.Any(v => v.Email == record.Email && v.Event.EventDate.Date == eventDate.Date))
                    continue;

                var evt = await _context.Events.FirstOrDefaultAsync(e =>
                    e.Location == location && e.EventDate.Date == eventDate.Date);

                if (evt == null)
                {
                    evt = new Event { Location = location, EventDate = eventDate };
                    _context.Events.Add(evt);
                    await _context.SaveChangesAsync();
                }

                TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;

                var veteran = new Veteran
                {
                    VeteranId = Guid.NewGuid().ToString(),
                    FirstName = record.FirstName ?? "",
                    LastName = record.LastName ?? "",
                    Email = record.Email,
                    PhoneNumber = record.PhoneNumber ?? "",
                    Gender = record.Gender ?? "Unknown",
                    StartOfService = DateTime.ParseExact(record.StartOfService, "yyyy/mm/dd", null),
                    EndOfService = DateTime.ParseExact(record.EndOfService, "yyyy/mm/dd", null),
                    DateOfBirth = DateTime.ParseExact(record.DateOfBirth, "yyyy/mm/dd", null),
                    BranchOfService = record.BranchOfService ?? "Unknown",
                    HealthConcerns = RemoveParenthesesContent(record.HealthConcerns ?? "None"),
                    AdditionalHealthInfo = record.AdditionalHealthInfo ?? "None",
                    HomeAddress = record.HomeAddress ?? "Unknown",
                    City = textInfo.ToTitleCase(record.City.ToLower()) ?? "Unknown",
                    State = NormalizeState(record.State ?? "Unknown"),
                    RelationshipStatus = record.RelationshipStatus ?? "Unknown",
                    MilitaryServiceStatus = record.MilitaryServiceStatus ?? "Unknown",
                    HighestRank = record.HighestRank ?? "Unknown",
                    DeploymentDetails = record.DeploymentDetails ?? "None",
                    PhysicalLimitations = record.PhysicalLimitations ?? "None",
                    NumberOfDeployments = record.NumberOfDeployments,
                    EventId = evt.EventId
                };

                _context.Veterans.Add(veteran);
                importedVeterans.Add(veteran);
            }

            await _context.SaveChangesAsync();
            return importedVeterans;
        }

        private static string NormalizeStateAndLocation(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return "Unknown";
            var parts = input.Split('-', StringSplitOptions.RemoveEmptyEntries);
            string location = parts.Length > 0 ? parts[0].Trim() : "Unknown";

            if (location.Contains(','))
            {
                var locParts = location.Split(',', StringSplitOptions.RemoveEmptyEntries);
                if (locParts.Length == 2)
                    return $"{locParts[0].Trim()}, {NormalizeState(locParts[1].Trim())}";
            }

            return NormalizeState(location);
        }

        private static string NormalizeState(string input)
        {
            var stateMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["alabama"] = "AL",
                ["al"] = "AL",
                ["alaska"] = "AK",
                ["ak"] = "AK",
                ["arizona"] = "AZ",
                ["az"] = "AZ",
                ["arkansas"] = "AR",
                ["ar"] = "AR",
                ["california"] = "CA",
                ["ca"] = "CA",
                ["colorado"] = "CO",
                ["co"] = "CO",
                ["connecticut"] = "CT",
                ["ct"] = "CT",
                ["delaware"] = "DE",
                ["de"] = "DE",
                ["florida"] = "FL",
                ["fl"] = "FL",
                ["georgia"] = "GA",
                ["ga"] = "GA",
                ["hawaii"] = "HI",
                ["hi"] = "HI",
                ["idaho"] = "ID",
                ["id"] = "ID",
                ["illinois"] = "IL",
                ["il"] = "IL",
                ["indiana"] = "IN",
                ["in"] = "IN",
                ["iowa"] = "IA",
                ["ia"] = "IA",
                ["kansas"] = "KS",
                ["ks"] = "KS",
                ["kentucky"] = "KY",
                ["ky"] = "KY",
                ["louisiana"] = "LA",
                ["la"] = "LA",
                ["maine"] = "ME",
                ["me"] = "ME",
                ["maryland"] = "MD",
                ["md"] = "MD",
                ["massachusetts"] = "MA",
                ["ma"] = "MA",
                ["michigan"] = "MI",
                ["mi"] = "MI",
                ["minnesota"] = "MN",
                ["mn"] = "MN",
                ["mississippi"] = "MS",
                ["ms"] = "MS",
                ["missouri"] = "MO",
                ["mo"] = "MO",
                ["montana"] = "MT",
                ["mt"] = "MT",
                ["nebraska"] = "NE",
                ["ne"] = "NE",
                ["nevada"] = "NV",
                ["nv"] = "NV",
                ["new hampshire"] = "NH",
                ["nh"] = "NH",
                ["new jersey"] = "NJ",
                ["nj"] = "NJ",
                ["new mexico"] = "NM",
                ["nm"] = "NM",
                ["new york"] = "NY",
                ["ny"] = "NY",
                ["north carolina"] = "NC",
                ["nc"] = "NC",
                ["north dakota"] = "ND",
                ["nd"] = "ND",
                ["ohio"] = "OH",
                ["oh"] = "OH",
                ["oklahoma"] = "OK",
                ["ok"] = "OK",
                ["oregon"] = "OR",
                ["or"] = "OR",
                ["pennsylvania"] = "PA",
                ["pa"] = "PA",
                ["rhode island"] = "RI",
                ["ri"] = "RI",
                ["south carolina"] = "SC",
                ["sc"] = "SC",
                ["south dakota"] = "SD",
                ["sd"] = "SD",
                ["tennessee"] = "TN",
                ["tn"] = "TN",
                ["texas"] = "TX",
                ["tx"] = "TX",
                ["utah"] = "UT",
                ["ut"] = "UT",
                ["vermont"] = "VT",
                ["vt"] = "VT",
                ["virginia"] = "VA",
                ["va"] = "VA",
                ["washington"] = "WA",
                ["wa"] = "WA",
                ["west virginia"] = "WV",
                ["wv"] = "WV",
                ["wisconsin"] = "WI",
                ["wi"] = "WI",
                ["wyoming"] = "WY",
                ["wy"] = "WY"
            };
            return stateMap.TryGetValue(input.Trim(), out var abbr) ? abbr : input.ToUpperInvariant();
        }

        private static string RemoveParenthesesContent(string input)
        {
            return Regex.Replace(input ?? "", @"\s*\([^)]*\)", "").Trim();
        }

        private class RagicVeteranDto
        {
            [JsonPropertyName("Veteran First Name:")]
            public string FirstName { get; set; }

            [JsonPropertyName("Veteran Last Name:")]
            public string LastName { get; set; }

            [JsonPropertyName("Veteran E-mail Address")]
            public string Email { get; set; }

            [JsonPropertyName("Veteran Cell Phone")]
            public string PhoneNumber { get; set; }

            [JsonPropertyName("Veteran Gender:")]
            public string Gender { get; set; }

            [JsonPropertyName("Approximate Start of Veterans Military Service")]
            public string StartOfService { get; set; }

            [JsonPropertyName("Approximate End of Veterans Military Service")]
            public string EndOfService { get; set; }

            [JsonPropertyName("Veteran Date of Birth")]
            public string DateOfBirth { get; set; }

            [JsonPropertyName("Branch of Service")]
            public string BranchOfService { get; set; }

            [JsonPropertyName("Please select any of the following health concerns that you")]
            public string HealthConcerns { get; set; }

            [JsonPropertyName("Please provide any additional information about the health concerns you described above:")]
            public string AdditionalHealthInfo { get; set; }

            [JsonPropertyName("Home Address")]
            public string HomeAddress { get; set; }

            [JsonPropertyName("City")]
            public string City { get; set; }

            [JsonPropertyName("State")]
            public string State { get; set; }

            [JsonPropertyName("What is your current relationship status?")]
            public string RelationshipStatus { get; set; }

            [JsonPropertyName("Please indicate your current military service status:")]
            public string MilitaryServiceStatus { get; set; }

            [JsonPropertyName("Highest military rank (current or at discharge/retirement):")]
            public string HighestRank { get; set; }

            [JsonPropertyName("Briefly describe your Combat Zone Deployments or Stateside Missions: Please include dates of deployment")]
            public string DeploymentDetails { get; set; }

            [JsonPropertyName("Please describe any physical limitations you have and any assistance/accommodations you will need during the retreat (e.g.")]
            public string PhysicalLimitations { get; set; }

            [JsonPropertyName("Number of Combat Zone Deployments")]
            public string NumberOfDeployments { get; set; }

            [JsonPropertyName("Which retreat date/location are you applying for?")]
            public string EventLocationDate { get; set; } // keep this single string
        }
    }
}