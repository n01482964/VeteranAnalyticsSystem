using System;
using System.Collections.Generic;
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
            var ragicRecords = ragicRecordsDictionary.Select(x=>x.Value).ToList();

            var importedVeterans = new List<Veteran>();

            int count = 0;
            foreach (var record in ragicRecords)
            {
                if (count >= 100) break;
                count++;

                if (string.IsNullOrWhiteSpace(record.Email))
                    continue;

                DateTime eventDate = record.EventDate == default ? DateTime.Now : record.EventDate;
                string location = NormalizeStateAndLocation(record.EventLocation);

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

                var veteran = new Veteran
                {
                    VeteranId = Guid.NewGuid().ToString(),
                    FirstName = record.FirstName ?? "",
                    LastName = record.LastName ?? "",
                    Email = record.Email,
                    PhoneNumber = record.PhoneNumber ?? "",
                    Gender = record.Gender ?? "Unknown",
                    StartOfService = record.StartOfService == default ? DateTime.Now : record.StartOfService,
                    EndOfService = record.EndOfService == default ? DateTime.Now : record.EndOfService,
                    DateOfBirth = record.DateOfBirth == default ? DateTime.Now : record.DateOfBirth,
                    BranchOfService = record.BranchOfService ?? "Unknown",
                    HealthConcerns = RemoveParenthesesContent(record.HealthConcerns ?? "None"),
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
                ["florida"] = "FL",
                ["fl"] = "FL",
                ["georgia"] = "GA",
                ["ga"] = "GA"
                // Add more states as needed
            };
            return stateMap.TryGetValue(input.Trim(), out var abbr) ? abbr : input.ToUpperInvariant();
        }

        private static string RemoveParenthesesContent(string input)
        {
            return Regex.Replace(input ?? "", @"\s*\([^)]*\)", "").Trim();
        }

        private class RagicVeteranDto
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            [JsonPropertyName("Veteran E-mail Address")]
            public string Email { get; set; }
            public string PhoneNumber { get; set; }
            public string Gender { get; set; }
            public DateTime StartOfService { get; set; }
            public DateTime EndOfService { get; set; }
            public DateTime DateOfBirth { get; set; }
            public string BranchOfService { get; set; }
            public string HealthConcerns { get; set; }
            public string AdditionalHealthInfo { get; set; }
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
        }
    }
}
