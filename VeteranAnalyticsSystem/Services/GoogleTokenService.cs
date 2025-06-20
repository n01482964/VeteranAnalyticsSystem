using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace VeteranAnalyticsSystem.Services
{
    public class GoogleTokenService
    {
        public static async Task<string> ExchangeCodeForTokenAsync(string clientId, string clientSecret, string redirectUri)
{
    var tokenEndpoint = "https://oauth2.googleapis.com/token"; // Replace with the actual endpoint

    var parameters = new Dictionary<string, string>
    {
        { "client_id", clientId }, // Replace with your client ID
        { "client_secret", clientSecret }, // Replace with your client secret
        { "redirect_uri", redirectUri }, // Replace with your redirect URI
        { "scope", "https://www.googleapis.com/auth/forms.responses.readonly" },
        { "response_type", "code" }
    };

    using (var client = new HttpClient())
    {
        var content = new FormUrlEncodedContent(parameters);

        var response = await client.PostAsync(tokenEndpoint, content);

        if (response.IsSuccessStatusCode)
        {
            var responseBody = await response.Content.ReadAsStringAsync();
            // Parse the JSON response to extract the access_token and refresh_token
            return responseBody; // Example: returns the raw JSON response
        }
        else
        {
                    // Handle error response
                    var responseBody = await response.Content.ReadAsStringAsync();

                    return null;
        }
    }
}

    }
}
