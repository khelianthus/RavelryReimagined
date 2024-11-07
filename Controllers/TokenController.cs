using API.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace API.Controllers;


/// <summary>
/// OAuth 2.0 flow. Start with authorize.
/// </summary>
[ApiController]
[Route("[controller]")]

public class TokenController : ControllerBase
{
    private readonly HttpClient _client;
    private readonly ILogger<TokenController> _logger;

    private readonly string clientId = Environment.GetEnvironmentVariable("clientId");
    private readonly string clientSecret = Environment.GetEnvironmentVariable("clientSecret");
    private readonly string redirectUri = Environment.GetEnvironmentVariable("redirect_uri");

    public TokenController(ILogger<TokenController> logger)
    {
        _client = new HttpClient();
        _logger = logger;
    }

    //Clients credentials flow
    //[HttpGet("access-token")]
    //public async Task<string> GetAccessTokenWithClientCredentials()
    //{

    //    // Add Basic Auth Header with client_id:client_secret base64-encoded
    //    var authToken = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{clientId}:{clientSecret}"));
    //    _client.DefaultRequestHeaders.Add("Authorization", $"Basic {authToken}");

    //    // Prepare request content
    //    var formData = new FormUrlEncodedContent(new[]
    //    {
    //    new KeyValuePair<string, string>("grant_type", "client_credentials")
    //});

    //    var tokenResponse = await _client.PostAsync("https://www.ravelry.com/oauth2/token", formData);
    //    if (!tokenResponse.IsSuccessStatusCode)
    //    {
    //        throw new Exception($"Error fetching token: {tokenResponse.StatusCode}");
    //    }

    //    var tokenContent = await tokenResponse.Content.ReadAsStringAsync();

    //    var tokenData = JsonSerializer.Deserialize<TokenResponse>(tokenContent);

    //    return tokenData.AccessToken; 
    //}

    /// <summary>
    /// Scope: profile-only
    /// </summary>
    /// <returns>Authorization url with scope profile-only</returns>

    [HttpGet("authorize")]
    public IActionResult AuthorizeUserProfileOnly()
    {
        var state = Guid.NewGuid().ToString("N"); // Generates a 32-character random string

        HttpContext.Session.SetString("oauth_state", state);

        var authorizationUrl = $"https://www.ravelry.com/oauth2/auth?response_type=code&client_id={clientId}&redirect_uri={redirectUri}&scope=profile-only&state={state}";
        return Ok(new { visitThisUrlInYourBrowser = authorizationUrl });
        //Redirect funkar inte via swagger
        //return Redirect(authorizationUrl);
    }

    /// <summary>
    /// Called after user is authorized by url above.
    /// </summary>
    /// <param name="code">Callback code</param>
    /// <param name="state">State</param>
    /// <returns></returns>
    [HttpGet("callback")]
    public async Task<IActionResult> Callback(string code, string state)
    {
        var originalState = HttpContext.Session.GetString("oauth_state");

        if (originalState == null || state != originalState)
        {
            return BadRequest("Invalid state parameter.");
        }

        if (string.IsNullOrEmpty(code))
        {
            return BadRequest("Authorization code is missing.");
        }

        var tokenResponse = await ExchangeProfileOnlyScopeCodeForToken(code);
        if (tokenResponse != null)
        {
            return Ok(tokenResponse);
        }

        return BadRequest("Failed to retrieve access token.");
    }

    private async Task<string> ExchangeProfileOnlyScopeCodeForToken(string code)
    {
        var authHeaderValue = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{clientId}:{clientSecret}"));

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authHeaderValue);
        _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));

        var requestData = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("grant_type", "authorization_code"),
            new KeyValuePair<string, string>("code", code),
            new KeyValuePair<string, string>("redirect_uri", redirectUri) 
        });

        var response = await _client.PostAsync("https://www.ravelry.com/oauth2/token", requestData);

        var tokenContent = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
        {
            var tokenData = JsonSerializer.Deserialize<TokenResponse>(tokenContent);
            return tokenData?.AccessToken;
        }
        else
        {
            _logger.LogError("Failed to retrieve token: " + tokenContent);
            return null;
        }
    }

    /// <summary>
    /// Get the json of the current user after browser authorization and access token has been exchanged. Uri reference: */Token/current-user?accessToken={accessToken}
    /// </summary>
    /// <param name="accessToken">Comes from ExchangeProfileOnlyScopeCodeForToken</param>
    /// <returns></returns>
    [HttpGet("current-user")]
    public async Task<IActionResult> GetCurrentUser([FromQuery] string accessToken)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "https://api.ravelry.com/current_user.json");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        HttpResponseMessage? response = null;

        if (string.IsNullOrEmpty(accessToken))
        {
            return BadRequest("Access token is required.");
        }

        try
        {
            response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode(); 

            var content = await response.Content.ReadAsStringAsync();

            return Ok(content);
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Request failed: {ex.Message}");
            return StatusCode((int)response.StatusCode, "Failed to retrieve current user data.");
        }
    }
}

