using System.Net.Http.Headers;
using System.Text.Json;

namespace API.Service;

/// <summary>
/// Service to handle calls to Ravelry API with Auth 2.0
/// </summary>
public class Auth2Service : IAuth2Service
{
    private readonly HttpClient httpClient;
    private readonly string? clientId = Environment.GetEnvironmentVariable("clientId");
    private readonly string? clientSecret = Environment.GetEnvironmentVariable("clientSecret");
    private readonly string? redirectUri = Environment.GetEnvironmentVariable("redirect_uri");

    public Auth2Service(HttpClient httpClient, IConfiguration configuration)
    {
        this.httpClient = httpClient;
    }

    public async Task<string> GetCurrentUser(string accessToken)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "https://api.ravelry.com/current_user.json");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        try
        {
            HttpResponseMessage response = await httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();

            using (JsonDocument doc = JsonDocument.Parse(content))
            {
                JsonElement userElement = doc.RootElement.GetProperty("user");

                var userDataString = userElement.GetRawText();

                return userDataString;
            }
        }
        catch (HttpRequestException ex)
        {
            return $"Failed to retrieve current user data: {ex.Message}";
        }
    }
}