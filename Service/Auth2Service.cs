using API.Models;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;
using System.Net.Http;

namespace API.Service;

/// <summary>
/// Service to handle Auth 2.0
/// Gets authenticated token that is valid for 24 hours
/// </summary>
public class Auth2Service : IAuth2Service
{
    private readonly HttpClient httpClient;
    private string clientId = "";
    private string clientSecret = "";
    private readonly string callbackUrl = "https://localhost:7290/callback";

    public Auth2Service(HttpClient httpClient, IConfiguration configuration)
    {
        this.httpClient = httpClient;
        this.clientId = configuration["Ravelry:ClientId"];
        this.clientSecret = configuration["Ravelry:ClientSecret"];
    }

    public async Task<string> GetAccessTokenAsync()
    {
        var authToken = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{clientId}:{clientSecret}"));
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authToken);

        var requestBody = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("grant_type", "client_credentials"),
            new KeyValuePair<string, string>("scope", "offline") 
        });

        var response = await httpClient.PostAsync("https://www.ravelry.com/oauth2/token", requestBody);

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Error fetching token: {response.StatusCode}");
        }

        var content = await response.Content.ReadAsStringAsync();
        var tokenData = JsonSerializer.Deserialize<TokenResponse>(content);
        return tokenData.AccessToken;
    }

    public async Task<string> GetCurrentUserAsync(string accessToken)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "https://api.ravelry.com/current_user.json");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await httpClient.SendAsync(request);
        //response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync();
        return responseString;
    }
}