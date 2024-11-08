using Newtonsoft.Json;
using RavelryReimagined.API.Models;
using System.Net.Http.Headers;
using System.Text.Json;

namespace API.Service;

/// <summary>
/// Service to handle calls to Ravelry API with Auth 2.0 access token
/// </summary>
public class RavelryService : IRavelryService
{
    private readonly HttpClient httpClient;
    private readonly ILogger<RavelryService> logger;
    private readonly string? clientId = Environment.GetEnvironmentVariable("clientId");
    private readonly string? clientSecret = Environment.GetEnvironmentVariable("clientSecret");
    private readonly string? redirectUri = Environment.GetEnvironmentVariable("redirect_uri");

    public RavelryService(HttpClient httpClient, IConfiguration configuration)
    {
        this.httpClient = httpClient;
    }

    public async Task<ProjectsData> ListProjects(string username, string accessToken)
    {

        if (string.IsNullOrEmpty(accessToken))
        {
            throw new InvalidOperationException("Access token is missing.");
        }

        var url = $"https://api.ravelry.com/projects/{username}/list.json";

        var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await httpClient.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            logger.LogError($"Error calling Ravelry API: {response.StatusCode} {response.ReasonPhrase}");
            throw new HttpRequestException($"Error calling Ravelry API: {response.StatusCode} {response.ReasonPhrase}");
        }

        var content = await response.Content.ReadAsStringAsync();

        var projectData = JsonConvert.DeserializeObject<ProjectsData>(content);

        return projectData;
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