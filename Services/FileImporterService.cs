using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using CsvHelper;
using CsvHelper.Configuration;
using ExcelDataReader;
using Microsoft.EntityFrameworkCore;
using VeteranAnalyticsSystem.Data;
using VeteranAnalyticsSystem.Models;

namespace VeteranAnalyticsSystem.Services
{
    public class FileImporterService
    {
        private readonly GratitudeAmericaDbContext _context;

        public FileImporterService(GratitudeAmericaDbContext context)
        {
            _context = context;
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
        }

        public List<Survey> ImportSurveys(string filePath, SurveyType surveyType)
        {
            var extension = Path.GetExtension(filePath).ToLowerInvariant();
            List<Survey> surveys;
            if (extension == ".csv")
            {
                surveys = ImportSurveysFromCsv(filePath, surveyType);
            }
            else if (extension == ".xlsx" || extension == ".xls")
            {
                surveys = ImportSurveysFromExcel(filePath, surveyType);
            }
            else
            {
                throw new NotSupportedException("File type not supported");
            }

            surveys = FilterDuplicateSurveys(surveys);
            return surveys;
        }

        private List<Survey> ImportSurveysFromCsv(string filePath, SurveyType surveyType)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture) { HasHeaderRecord = true };

            using var reader = new StreamReader(filePath);
            using var csv = new CsvReader(reader, config);
            var records = new List<Dictionary<string, string>>();

            csv.Read();
            csv.ReadHeader();
            var headers = csv.HeaderRecord;

            while (csv.Read())
            {
                var dict = new Dictionary<string, string>();
                foreach (var header in headers)
                    dict[header] = csv.GetField(header);
                records.Add(dict);
            }

            return records.Select(row =>
            {
                var email = row.FirstOrDefault(kvp => kvp.Key.Equals("Email", StringComparison.OrdinalIgnoreCase) ||
                                                       kvp.Key.Equals("Email Address", StringComparison.OrdinalIgnoreCase)).Value ?? string.Empty;
                var dateString = row.FirstOrDefault(kvp => kvp.Key.Contains("Timestamp", StringComparison.OrdinalIgnoreCase) ||
                                                           kvp.Key.Equals("SubmissionDate", StringComparison.OrdinalIgnoreCase)).Value;
                var submissionDate = DateTime.TryParse(dateString, out var parsedDate) ? parsedDate : DateTime.Now;

                return new Survey
                {
                    Email = email,
                    SubmissionDate = submissionDate,
                    SurveyType = surveyType,
                    EventId = 0,
                    Responses = row.Where(kvp => !kvp.Key.Contains("Email", StringComparison.OrdinalIgnoreCase) &&
                                                  !kvp.Key.Contains("Timestamp", StringComparison.OrdinalIgnoreCase))
                                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value)
                };
            }).ToList();
        }

        private List<Survey> ImportSurveysFromExcel(string filePath, SurveyType surveyType)
        {
            var surveys = new List<Survey>();
            using var stream = File.Open(filePath, FileMode.Open, FileAccess.Read);
            using var reader = ExcelReaderFactory.CreateReader(stream);

            bool headerRow = true;
            List<string> headers = new();

            while (reader.Read())
            {
                if (headerRow)
                {
                    for (int i = 0; i < reader.FieldCount; i++)
                        headers.Add(reader.GetValue(i)?.ToString() ?? $"Q{i}");
                    headerRow = false;
                    continue;
                }

                var responses = new Dictionary<string, string>();
                for (int col = 0; col < reader.FieldCount; col++)
                {
                    string question = col < headers.Count ? headers[col] : $"Q{col}";
                    string answer = reader.GetValue(col)?.ToString() ?? string.Empty;
                    responses[question] = answer;
                }

                string email = responses.FirstOrDefault(kvp => kvp.Key.Contains("Email", StringComparison.OrdinalIgnoreCase)).Value ?? string.Empty;
                string dateString = responses.FirstOrDefault(kvp => kvp.Key.Contains("Timestamp", StringComparison.OrdinalIgnoreCase) ||
                                                                     kvp.Key.Contains("SubmissionDate", StringComparison.OrdinalIgnoreCase)).Value;
                var submissionDate = DateTime.TryParse(dateString, out var parsedDate) ? parsedDate : DateTime.Now;

                surveys.Add(new Survey
                {
                    Email = email,
                    SubmissionDate = submissionDate,
                    SurveyType = surveyType,
                    EventId = 0,
                    Responses = responses
                        .Where(kvp => !kvp.Key.Contains("Email", StringComparison.OrdinalIgnoreCase) &&
                                      !kvp.Key.Contains("Timestamp", StringComparison.OrdinalIgnoreCase))
                        .ToDictionary(kvp => kvp.Key, kvp => kvp.Value)
                });
            }

            return surveys;
        }
        public List<Veteran> ImportVeteransFromExcel(string filePath)
        {
            var veterans = new List<Veteran>();
            using var stream = File.Open(filePath, FileMode.Open, FileAccess.Read);
            using var reader = ExcelReaderFactory.CreateReader(stream);

            var header = new List<string>();
            bool isFirstRow = true;

            while (reader.Read())
            {
                if (isFirstRow)
                {
                    for (int i = 0; i < reader.FieldCount; i++)
                        header.Add(reader.GetValue(i)?.ToString() ?? $"Col{i}");
                    isFirstRow = false;
                    continue;
                }

                try
                {
                    string retreatInfo = reader.GetValue(header.IndexOf("Which retreat date/location are you applying for?"))?.ToString() ?? "";
                    string location = "Unknown";
                    DateTime eventDate = DateTime.MinValue;

                    if (!string.IsNullOrWhiteSpace(retreatInfo) && retreatInfo.Contains("-"))
                    {
                        var parts = retreatInfo.Split('-');
                        location = parts[0].Trim();

                        // If location includes a comma and state, normalize the state
                        if (location.Contains(","))
                        {
                            var locParts = location.Split(',', StringSplitOptions.RemoveEmptyEntries);
                            if (locParts.Length == 2)
                            {
                                string city = locParts[0].Trim();
                                string statePart = NormalizeState(locParts[1].Trim());
                                location = $"{city}, {statePart}";
                            }
                        }
                        else
                        {
                            // Optional fallback: Normalize if state abbreviation might be the whole location
                            location = NormalizeState(location);
                        }

                        string dateStr = retreatInfo.Substring(retreatInfo.IndexOf('-') + 1).Trim();
                        eventDate = ParseEventDateFromRange(dateStr);
                    }

                    var evt = _context.Events.FirstOrDefault(e =>
                        e.Location == location && e.EventDate.Date == eventDate.Date);

                    if (evt == null)
                    {
                        evt = new Event { Location = location, EventDate = eventDate };
                        _context.Events.Add(evt);
                        _context.SaveChanges();
                    }

                    var vet = new Veteran
                    {
                        VeteranId = Guid.NewGuid().ToString(),
                        FirstName = reader.GetValue(header.IndexOf("Veteran First Name:"))?.ToString() ?? "",
                        LastName = reader.GetValue(header.IndexOf("Veteran Last Name:"))?.ToString() ?? "",
                        Email = reader.GetValue(header.IndexOf("Veteran E-mail Address"))?.ToString() ?? "",
                        PhoneNumber = reader.GetValue(header.IndexOf("Veteran Cell Phone"))?.ToString() ?? "",
                        Gender = reader.GetValue(header.IndexOf("Veteran Gender:"))?.ToString() ?? "",
                        StartOfService = DateTime.TryParse(reader.GetValue(header.IndexOf("Approximate Start of Veterans Military Service"))?.ToString(), out var start) ? start : DateTime.Now,
                        EndOfService = DateTime.TryParse(reader.GetValue(header.IndexOf("Approximate End of Veterans Military Service"))?.ToString(), out var end) ? end : DateTime.Now,
                        DateOfBirth = DateTime.TryParse(reader.GetValue(header.IndexOf("Veteran Date of Birth"))?.ToString(), out var dob) ? dob : DateTime.Now,
                        HomeAddress = reader.GetValue(header.IndexOf("Home Address"))?.ToString() ?? "Unknown",
                        City = reader.GetValue(header.IndexOf("City"))?.ToString() ?? "Unknown",
                        State = NormalizeState(reader.GetValue(header.IndexOf("State"))?.ToString() ?? "Unknown"),
                        RelationshipStatus = reader.GetValue(header.IndexOf("What is your current relationship status?"))?.ToString() ?? "Unknown",
                        MilitaryServiceStatus = reader.GetValue(header.IndexOf("Please indicate your current military service status:"))?.ToString() ?? "Unknown",
                        HighestRank = reader.GetValue(header.IndexOf("Highest military rank (current or at discharge/retirement):"))?.ToString() ?? "Unknown",
                        BranchOfService = reader.GetValue(header.IndexOf("Branch of Service"))?.ToString() ?? "Unknown",
                        DeploymentDetails = reader.GetValue(header.IndexOf("Briefly describe your Combat Zone Deployments or Stateside Missions: Please include dates of deployment"))?.ToString() ?? "None",
                        HealthConcerns = RemoveParenthesesContent(reader.GetValue(header.IndexOf("Please select any of the following health concerns that you"))?.ToString() ?? "None"),
                        AdditionalHealthInfo = reader.GetValue(header.IndexOf("Please provide any additional information about the health concerns you described above:"))?.ToString() ?? "None",
                        PhysicalLimitations = reader.GetValue(header.IndexOf("Please describe any physical limitations you have and any assistance/accommodations you will need during the retreat (e.g."))?.ToString() ?? "None",
                        //NumberOfDeployments = reader.GetValue(header.IndexOf("Number of Combat Zone Deployments").ToString()),
                        EventId = evt.EventId
                    };

                    veterans.Add(vet);
                }
                catch
                {
                    continue;
                }
            }

            return veterans;
        }

        private static string NormalizeState(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return "UNKNOWN";

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

            return stateMap.TryGetValue(input.Trim().ToLowerInvariant(), out var abbr)
                ? abbr
                : input.ToUpperInvariant();
        }


        private DateTime ParseEventDateFromRange(string dateStr)
        {
            if (string.IsNullOrWhiteSpace(dateStr)) return DateTime.MinValue;

            try
            {
                // Normalize spacing between month and day
                dateStr = Regex.Replace(dateStr, @"([a-zA-Z]+)(\d)", "$1 $2").Trim();
                dateStr = Regex.Replace(dateStr, @"\s*,\s*", ", ", RegexOptions.None);

                // Handle formats like "February 28-March 3, 2019"
                var multiMonthMatch = Regex.Match(dateStr, @"([A-Za-z]+ \d+).*(\d{4})");
                if (multiMonthMatch.Success)
                {
                    string monthDay = multiMonthMatch.Groups[1].Value;
                    string year = multiMonthMatch.Groups[2].Value;
                    string formatted = $"{monthDay}, {year}";

                    if (DateTime.TryParse(formatted, out var parsedMultiMonth))
                        return parsedMultiMonth;
                }

                // Fallback for normal format: e.g., "February 20-22, 2021"
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


        private List<Survey> FilterDuplicateSurveys(List<Survey> surveys)
        {
            var uniqueSurveys = new List<Survey>();
            foreach (var survey in surveys)
            {
                bool duplicate = _context.Surveys.Any(existing =>
                    existing.Email.ToLower() == survey.Email.ToLower() &&
                    EF.Functions.DateDiffDay(existing.SubmissionDate, survey.SubmissionDate) == 0 &&
                    existing.SurveyType == survey.SurveyType);
                if (!duplicate) uniqueSurveys.Add(survey);
            }
            return uniqueSurveys;
        }

        private static string RemoveParenthesesContent(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return input;
            return Regex.Replace(input, @"\s*\([^)]*\)", "").Trim();
        }

    }

    public sealed class SurveyCsvMap : ClassMap<Survey>
    {
        public SurveyCsvMap()
        {
            Map(m => m.Email).Name("Email");
            Map(m => m.SubmissionDate).Name("SubmissionDate");
        }
    }
}