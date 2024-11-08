using API.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]

public class TokenController : ControllerBase
{
    private readonly HttpClient httpClient;
    private readonly ILogger<TokenController> logger;

    private readonly string clientId = Environment.GetEnvironmentVariable("clientId")!;
    private readonly string clientSecret = Environment.GetEnvironmentVariable("clientSecret")!;
    private readonly string redirectUri = Environment.GetEnvironmentVariable("redirect_uri")!;

    public TokenController(ILogger<TokenController> logger, HttpClient httpClient)
    {
        this.logger = logger;
        this.httpClient = httpClient;
    }

    /// <summary>
    /// Scope: profile-only
    /// </summary>
    /// <returns>Authorization url with scope profile-only</returns>

    [HttpGet("authorize-profile-only-scope")]
    public IActionResult AuthorizeUserProfileOnly()
    {
        var state = Guid.NewGuid().ToString("N"); 

        HttpContext.Session.SetString("oauth_state", state);

        var authorizationUrl = $"https://www.ravelry.com/oauth2/auth?response_type=code&client_id={clientId}&redirect_uri={redirectUri}&scope=profile-only&state={state}";
        return Ok(new { visitThisUrlInYourBrowser = authorizationUrl });
        //Redirect funkar inte via swagger
        //return Redirect(authorizationUrl);
    }

    [HttpGet("authorize-offline-scope")]
    public IActionResult AuthorizeUserOfflineScope()
    {
        var state = Guid.NewGuid().ToString("N");

        HttpContext.Session.SetString("oauth_state", state);

        var authorizationUrl = $"https://www.ravelry.com/oauth2/auth?response_type=code&client_id={clientId}&redirect_uri={redirectUri}&scope=offline&state={state}";
        return Ok(new { visitThisUrlInYourBrowser = authorizationUrl });
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

    private async Task<object?> ExchangeProfileOnlyScopeCodeForToken(string code)
    {
        var authHeaderValue = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{clientId}:{clientSecret}"));

        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authHeaderValue);
        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));

        var requestData = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("grant_type", "authorization_code"),
            new KeyValuePair<string, string>("code", code),
            new KeyValuePair<string, string>("redirect_uri", redirectUri),
            new KeyValuePair<string, string>("scope", "offline")  
        });

        var response = await httpClient.PostAsync("https://www.ravelry.com/oauth2/token", requestData);

        var tokenContent = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
        {
            var tokenData = JsonSerializer.Deserialize<TokenResponse>(tokenContent);
            return tokenData;
        }
        else
        {
            logger.LogError("Failed to retrieve token: " + tokenContent);
            return null;
        }
    }
}

#region

//Clients credentials flow
//[HttpGet("access-token")]
//public async Task<string> GetAccessTokenWithClientCredentials()
//{

//    var authToken = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{clientId}:{clientSecret}"));
//    _client.DefaultRequestHeaders.Add("Authorization", $"Basic {authToken}");

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

#endregion