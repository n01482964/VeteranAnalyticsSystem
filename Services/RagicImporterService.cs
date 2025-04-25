using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VeteranAnalyticsSystem.Data;
using VeteranAnalyticsSystem.Models;

namespace VeteranAnalyticsSystem.Services
{
    public class RagicImporterService
    {
        private readonly GratitudeAmericaDbContext _context;
        private readonly HttpClient _httpClient;

        private const string RagicApiUrl = "https://www.ragic.com/YOUR_ACCOUNT/database/1";
        private const string ApiKey = "bEh2YUVIYm5PRHVkb2lMQTlnc3YxTHBTblF5TWQ1ZGZkc2xJYU9ENWZIOVA1Y3BGWXJkS2ZtOUJKQys3cEp2cg==";

        public RagicImporterService(GratitudeAmericaDbContext context, HttpClient httpClient)
        {
            _context = context;
            _httpClient = httpClient;
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", ApiKey);
        }

        public async Task<List<Veteran>> ImportVeteransBatchAsync(int skip = 0)
        {
            var requestUrl = $"{RagicApiUrl}?api&limit=100&offset={skip}";
            var response = await _httpClient.GetAsync(requestUrl);

            if (!response.IsSuccessStatusCode)
                throw new Exception("Failed to fetch data from Ragic.");

            var json = await response.Content.ReadAsStringAsync();
            var ragicRecords = JsonSerializer.Deserialize<List<RagicVeteranDto>>(json);

            var importedVeterans = new List<Veteran>();

            foreach (var record in ragicRecords)
            {
                if (string.IsNullOrWhiteSpace(record.Email)) continue;
                if (_context.Veterans.Any(v => v.Email == record.Email && v.Event.EventDate == record.EventDate)) continue;

                string location = NormalizeStateAndLocation(record.EventLocation);
                DateTime eventDate = record.EventDate == DateTime.MinValue ? DateTime.Now : record.EventDate;

                var evt = await _context.Events.FirstOrDefaultAsync(e => e.Location == location && e.EventDate.Date == eventDate.Date);
                if (evt == null)
                {
                    evt = new Event { Location = location, EventDate = eventDate };
                    _context.Events.Add(evt);
                    await _context.SaveChangesAsync();
                }

                var veteran = new Veteran
                {
                    VeteranId = Guid.NewGuid().ToString(),
                    FirstName = record.FirstName ?? "",
                    LastName = record.LastName ?? "",
                    Email = record.Email ?? "",
                    PhoneNumber = record.PhoneNumber ?? "",
                    Gender = record.Gender ?? "",
                    StartOfService = record.StartOfService == DateTime.MinValue ? DateTime.Now : record.StartOfService,
                    EndOfService = record.EndOfService == DateTime.MinValue ? DateTime.Now : record.EndOfService,
                    DateOfBirth = record.DateOfBirth == DateTime.MinValue ? DateTime.Now : record.DateOfBirth,
                    BranchOfService = record.BranchOfService ?? "",
                    HealthConcerns = RemoveParenthesesContent(record.HealthConcerns ?? ""),
                    AdditionalHealthInfo = record.AdditionalHealthInfo ?? "None",
                    HomeAddress = record.HomeAddress ?? "Unknown",
                    City = record.City ?? "Unknown",
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
            string locationPart = parts.Length > 0 ? parts[0].Trim() : "Unknown";

            if (locationPart.Contains(","))
            {
                var locParts = locationPart.Split(',', StringSplitOptions.RemoveEmptyEntries);
                if (locParts.Length == 2)
                {
                    string city = locParts[0].Trim();
                    string state = NormalizeState(locParts[1].Trim());
                    return $"{city}, {state}";
                }
            }
            return NormalizeState(locationPart);
        }

        private static string NormalizeState(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return "UNKNOWN";

            var stateMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["florida"] = "FL",
                ["fl"] = "FL",
                ["georgia"] = "GA",
                ["ga"] = "GA"
                // Add other states as needed
            };

            return stateMap.TryGetValue(input.Trim().ToLowerInvariant(), out var abbr)
                ? abbr
                : input.ToUpperInvariant();
        }

        private DateTime ParseEventDateFromRange(string dateStr)
        {
            if (string.IsNullOrWhiteSpace(dateStr)) return DateTime.MinValue;

            try
            {
                dateStr = Regex.Replace(dateStr, @"([a-zA-Z]+)(\d)", "$1 $2").Trim();
                dateStr = Regex.Replace(dateStr, @"\s*,\s*", ", ", RegexOptions.None);

                var multiMonthMatch = Regex.Match(dateStr, @"([A-Za-z]+ \d+).*(\d{4})");
                if (multiMonthMatch.Success)
                {
                    string monthDay = multiMonthMatch.Groups[1].Value;
                    string year = multiMonthMatch.Groups[2].Value;
                    string formatted = $"{monthDay}, {year}";

                    if (DateTime.TryParse(formatted, out var parsedMultiMonth))
                        return parsedMultiMonth;
                }

                var parts = dateStr.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length < 2) return DateTime.MinValue;

                string month = parts[0];
                string dayPart = parts[1];
                string yearPart = parts.Length >= 3 ? parts[2].Trim(',') : DateTime.Now.Year.ToString();
                string startDay = dayPart.Contains('-') ? dayPart.Split('-')[0] : dayPart;

                string formattedDate = $"{month} {startDay}, {yearPart}";
                return DateTime.TryParse(formattedDate, out var fallbackDate) ? fallbackDate : DateTime.MinValue;
            }
            catch
            {
                return DateTime.MinValue;
            }
        }

        private static string RemoveParenthesesContent(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return input;
            return Regex.Replace(input, @"\s*\([^)]*\)", "").Trim();
        }

        private class RagicVeteranDto
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Email { get; set; }
            public string PhoneNumber { get; set; }
            public string Gender { get; set; }
            public DateTime StartOfService { get; set; }
            public DateTime EndOfService { get; set; }
            public DateTime DateOfBirth { get; set; }
            public string BranchOfService { get; set; }
            public string HealthConcerns { get; set; }
            public string AdditionalHealthInfo { get; set; }  // ✅ Added this property
            public string HomeAddress { get; set; }
            public string City { get; set; }
            public string State { get; set; }
            public string RelationshipStatus { get; set; }
            public string MilitaryServiceStatus { get; set; }
            public string HighestRank { get; set; }
            public string DeploymentDetails { get; set; }
            public string PhysicalLimitations { get; set; }
            public int NumberOfDeployments { get; set; }
            public string EventLocation { get; set; }
            public DateTime EventDate { get; set; }
            public string EventDateRaw { get; set; }  // Optional, if you're handling date parsing manually
        }

    }
}
