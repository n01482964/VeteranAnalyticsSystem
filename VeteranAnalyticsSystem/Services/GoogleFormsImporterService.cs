using VeteranAnalyticsSystem.Data;
using VeteranAnalyticsSystem.Models;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Sheets.v4;

namespace VeteranAnalyticsSystem.Services;

public class GoogleFormsImporterService
{
    private readonly GratitudeAmericaDbContext _context;
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    private readonly string PreFormId;
    private readonly string PostFormId;

    public GoogleFormsImporterService(GratitudeAmericaDbContext context, HttpClient httpClient, IConfiguration configuration)
    {
        _context = context;
        _httpClient = httpClient;
        _configuration = configuration;

        PreFormId = configuration.GetValue<string>("PreFormId");
        PostFormId = configuration.GetValue<string>("PostFormId");

        if (string.IsNullOrWhiteSpace(PreFormId)
            || string.IsNullOrWhiteSpace(PostFormId)
            )
            throw new Exception("Error: Pre or Post Form ID is null");

    }

    public async Task<List<Survey>> ImportSurveysFromGoogleFormsAsync()
    {
        var preSurveys = await ImportFromFormAsync(PreFormId, SurveyType.PreRetreat);
        //var postSurveys = await ImportFromFormAsync(PostFormId, SurveyType.PostRetreat);
        //return preSurveys.Concat(postSurveys).ToList();
        return preSurveys.ToList();
    }

    private async Task<List<Survey>> ImportFromFormAsync(string formId, SurveyType surveyType)
    {
        await Temp();

        return new List<Survey>();

        //_httpClient.DefaultRequestHeaders.Authorization =
        //    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        //var response = await _httpClient.GetAsync($"https://forms.googleapis.com/v1/forms/{formId}/responses");
        //if (!response.IsSuccessStatusCode)
        //    throw new Exception($"Failed to fetch responses from form {formId}. Status: {response.StatusCode}");

        //var json = await response.Content.ReadAsStringAsync();
        //var parsed = JsonDocument.Parse(json);

        //var surveys = new List<Survey>();

        //if (!parsed.RootElement.TryGetProperty("responses", out var responseArray))
        //    return surveys;

        //foreach (var item in responseArray.EnumerateArray())
        //{
        //    var dto = new GoogleFormsSurveyDto
        //    {
        //        ResponseId = item.GetProperty("responseId").GetString(),
        //        CreateTime = item.GetProperty("createTime").GetDateTime(),
        //        RespondentEmail = item.GetProperty("respondentEmail").GetString()
        //    };

        //    if (item.TryGetProperty("answers", out var answers))
        //    {
        //        foreach (var ans in answers.EnumerateObject())
        //        {
        //            var questionId = ans.Name;
        //            var label = QuestionMap.GetValueOrDefault(questionId, questionId);
        //            var value = ans.Value.GetProperty("textAnswers").GetProperty("answers")[0].GetProperty("value").GetString();
        //            dto.Answers[label] = value;
        //        }
        //    }

        //    if (string.IsNullOrWhiteSpace(dto.RespondentEmail))
        //        continue;

        //    var survey = new Survey
        //    {
        //        Email = dto.RespondentEmail,
        //        SubmissionDate = dto.CreateTime,
        //        SurveyType = surveyType,
        //        Responses = dto.Answers
        //    };

        //    _context.Surveys.Add(survey);
        //    surveys.Add(survey);
        //}

        //await _context.SaveChangesAsync();
        //return surveys;


    }

    private async Task Temp()
    {
        string[] Scopes = { SheetsService.Scope.SpreadsheetsReadonly };
        string ApplicationName = "Veteran Analytics System";
        string Range = "Form Responses 1!A1:N"; // 1!A1:R for postreat

        GoogleCredential credential;
        using (var stream = new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
        {
            credential = GoogleCredential.FromStream(stream).CreateScoped(Scopes);
        }

        var service = new SheetsService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential,
            ApplicationName = ApplicationName,
        });

        SpreadsheetsResource.ValuesResource.GetRequest request =
        service.Spreadsheets.Values.Get(PreFormId, Range);

        ValueRange response = await request.ExecuteAsync();
        var values = response.Values;

        if (values != null && values.Count > 0)
        {
            foreach (var row in values)
            {
                Console.WriteLine(string.Join(", ", row));
            }
        }
        else
        {
            Console.WriteLine("No data found.");
        }

    }

    // Inline DTO
    private class GoogleFormsSurveyDto
    {
        public string ResponseId { get; set; }
        public DateTime CreateTime { get; set; }
        public string RespondentEmail { get; set; }
        public Dictionary<string, string> Answers { get; set; } = new();
    }

    // ID → Label mapping
    private static readonly Dictionary<string, string> QuestionMap = new()
    {
        { "32cfe45b", "Conflict Resolution Quality" },
        { "3ff850ce", "Outlook Post MSR" },
        { "6fd79ba8", "Wants to Continue Growth" },
        { "20a36aa6", "Overall Benefit" },
        { "3316b3ae", "Sharing Past Struggles" },
        { "459c56b6", "Comfort Zone Tendency" },
        { "3400ca2f", "Rating Score" },
        { "1374e3e8", "Retreat ID" },
        { "21d39a42", "Connection Level" },
        { "3302ce16", "Staff Comments" }
    };
}