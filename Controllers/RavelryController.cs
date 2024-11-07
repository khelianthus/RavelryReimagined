using API.Service;
using Microsoft.AspNetCore.Mvc;
using RavelryReimagined.API.Models;
using System.Text.Json;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
public class RavelryController : Controller
{
    private readonly IAuth2Service auth2Service;
    public RavelryController(IAuth2Service auth2Service)
    {
        this.auth2Service = auth2Service;
    }

    /// <summary>
    /// Get current-user.json from Ravelry API
    /// </summary>
    /// <param name="accessToken">Use your authenticated accesstoken from token controller</param>
    /// <returns>current-user.json</returns>
    [HttpGet("current-user")]
    public async Task<IActionResult> GetCurrentUser(string accessToken)
    {
        try
        {
            var userData = await auth2Service.GetCurrentUser(accessToken);

            var mappedUser = JsonSerializer.Deserialize<CurrentUser>(userData);
            return Ok(mappedUser);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "An error occurred: " + ex.Message);
        }
    }
}
