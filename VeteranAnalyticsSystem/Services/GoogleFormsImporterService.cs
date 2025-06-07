using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VeteranAnalyticsSystem.Data;
using VeteranAnalyticsSystem.Models;
using System.Text.Json;

namespace VeteranAnalyticsSystem.Services
{
    public class GoogleFormsImporterService
    {
        private readonly GratitudeAmericaDbContext _context;
        private readonly HttpClient _httpClient;

        private const string PreretreatFormId = "YOUR_PRE_FORM_ID";
        private const string PostretreatFormId = "YOUR_POST_FORM_ID";

        public GoogleFormsImporterService(GratitudeAmericaDbContext context, HttpClient httpClient)
        {
            _context = context;
            _httpClient = httpClient;
        }

        public async Task<List<Survey>> ImportSurveysFromGoogleFormsAsync()
        {
            var preSurveys = await ImportFromFormAsync(PreretreatFormId, SurveyType.PreRetreat);
            var postSurveys = await ImportFromFormAsync(PostretreatFormId, SurveyType.PostRetreat);

            return preSurveys.Concat(postSurveys).ToList();
        }

        private async Task<List<Survey>> ImportFromFormAsync(string formId, SurveyType surveyType)
        {
            var accessToken = await GetAccessTokenAsync(); // OAuth 2.0 token
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _httpClient.GetAsync($"https://forms.googleapis.com/v1/forms/{formId}/responses");

            if (!response.IsSuccessStatusCode)
                throw new Exception($"Failed to fetch responses from form {formId}. Status: {response.StatusCode}");

            var json = await response.Content.ReadAsStringAsync();
            var parsed = JsonDocument.Parse(json);

            var surveys = new List<Survey>();

            if (!parsed.RootElement.TryGetProperty("responses", out var responseArray))
                return surveys;

            foreach (var item in responseArray.EnumerateArray())
            {
                string email = "";
                var responses = new Dictionary<string, string>();

                if (item.TryGetProperty("answers", out var answers))
                {
                    foreach (var ans in answers.EnumerateObject())
                    {
                        var question = ans.Name;
                        var value = ans.Value
                            .GetProperty("textAnswers")
                            .GetProperty("answers")[0]
                            .GetProperty("value")
                            .GetString();

                        if (question.ToLower().Contains("email"))
                            email = value;

                        responses[question] = value;
                    }
                }

                if (string.IsNullOrWhiteSpace(email))
                    continue;

                var submissionDate = item.GetProperty("createTime").GetDateTime();

                var survey = new Survey
                {
                    Email = email,
                    SurveyType = surveyType,
                    SubmissionDate = submissionDate,
                    Responses = responses
                };

                _context.Surveys.Add(survey);
                surveys.Add(survey);
            }

            await _context.SaveChangesAsync();
            return surveys;
        }

        private async Task<string> GetAccessTokenAsync()
        {
            // TODO: Implement OAuth 2.0 flow using Google.Apis.Auth library or service account
            throw new NotImplementedException("Google OAuth2 token retrieval not implemented.");
        }
    }
}
