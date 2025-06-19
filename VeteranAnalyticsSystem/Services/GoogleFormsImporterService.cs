using VeteranAnalyticsSystem.Data;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Sheets.v4;
using VeteranAnalyticsSystem.Extensions;
using VeteranAnalyticsSystem.Contracts;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using VeteranAnalyticsSystem.Models.Core;

namespace VeteranAnalyticsSystem.Services;

public class GoogleFormsImporterService(
    GratitudeAmericaDbContext context,
    IConfiguration configuration,
    IGoogleFormCredentialService googleFormCredentialService) : IGoogleFormsImporterService
{
    private readonly string PreFormId = configuration.GetRequiredValue<string>("PreFormId");
    private readonly string PostFormId = configuration.GetRequiredValue<string>("PostFormId");

    public async Task<int> ImportForms()
    {
        var preSurveysCount = await ImportForm(SurveyType.PreRetreat);
        var postSurveysCount = await ImportForm(SurveyType.PostRetreat);

        context.SyncRecords.Add(new SyncRecord
        {
            TimeStamp = DateTime.UtcNow,
            SyncType = Models.Enums.SyncTypes.GoogleForms
        });
        await context.SaveChangesAsync();

        return preSurveysCount + postSurveysCount;
    }

    private async Task<int> ImportForm(SurveyType surveyType)
    {
        try
        {
            var credentials = await googleFormCredentialService.DownloadCredentials();

            string[] scopes = { SheetsService.Scope.SpreadsheetsReadonly };
            string range = surveyType == SurveyType.PreRetreat ? "Form Responses 1!A1:N" : "Form Responses 1!A1:R";

            using var stream = new MemoryStream(credentials);
            var credential = GoogleCredential.FromStream(stream).CreateScoped(scopes);

            var service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "Veteran Analytics System"
            });

            var formId = surveyType == SurveyType.PreRetreat ? PreFormId : PostFormId;

            SpreadsheetsResource.ValuesResource.GetRequest request = service.Spreadsheets.Values.Get(formId, range);

            ValueRange response = await request.ExecuteAsync();
            var values = response.Values;

            var surveys = new List<Survey>();

            if (values != null && values.Count > 1)
            {
                foreach (var row in values)
                {
                    Console.WriteLine(string.Join(",", row));

                    switch (surveyType)
                    {
                        case SurveyType.PreRetreat:
                            surveys.Add(HandlePreRetreat(row));
                            break;
                        case SurveyType.PostRetreat:
                            surveys.Add(HandlePostRetreat(row));
                            break;
                    }
                }
            }

            var events = await context.Events.ToListAsync();

            foreach (var survey in surveys)
            {
                var closestEvent = events.Where(e => e.EventDate <= survey.SubmissionDate).OrderByDescending(e => e.EventDate).FirstOrDefault();
                if (closestEvent != null)
                {
                    survey.EventId = closestEvent.EventId;
                }
            }

            context.Surveys.AddRange(surveys);
            await context.SaveChangesAsync();

            return surveys.Count;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during temporary operation: {ex.Message}");
        }

        return 0;
    }

    private static Survey HandlePreRetreat(IList<object> row)
    {
        var timeStamp = row[0].ToString();
        var email = row[1].ToString();
        var identifier = row[2].ToString();
        DateTime? submissionDate = !string.IsNullOrWhiteSpace(timeStamp) ? DateTime.Parse(timeStamp) : null;

        return new Survey
        {
            Email = email,
            SelfIdentifier = identifier,
            SubmissionDate = submissionDate,
            SurveyType = SurveyType.PreRetreat,
            EmotionalConnection = row.Count > 3 ? row[3].ToString() : null,
            ConflictResolution = row.Count > 4 ? row[4].ToString() : null,
            PastStruggles = row.Count > 5 ? row[5].ToString() : null,
            ComfortZone = row.Count > 6 ? row[6].ToString() : null,
            Rating = row.Count > 7 ? row[7].ToString() : null,
            ResponsesJson = JsonSerializer.Serialize(row)
        };
    }

    private static Survey HandlePostRetreat(IList<object> row)
    {
        var timeStamp = row[0].ToString();
        var email = row[1].ToString();
        var identifier = row[2].ToString();
        DateTime? submissionDate = !string.IsNullOrWhiteSpace(timeStamp) ? DateTime.Parse(timeStamp) : null;

        return new Survey
        {
            Email = email,
            SelfIdentifier = identifier,
            SubmissionDate = submissionDate,
            SurveyType = SurveyType.PreRetreat,
            EmotionalConnection = row.Count > 3 ? row[3].ToString() : null,
            ConflictResolution = row.Count > 4 ? row[4].ToString() : null,
            PastStruggles = row.Count > 5 ? row[5].ToString() : null,
            ComfortZone = row.Count > 6 ? row[6].ToString() : null,
            Rating = row.Count > 7 ? row[7].ToString() : null,
            ExperienceRating = row.Count > 8 ? row[8].ToString() : null,
            LifeImpact = row.Count > 9 ? row[9].ToString() : null,
            Recommendation = row.Count > 10 ? row[10].ToString() : null,
            Feedback = row.Count > 11 ? row[11].ToString() : null,
            ResponsesJson = JsonSerializer.Serialize(row)
        };
    }
}