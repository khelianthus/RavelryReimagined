using API.Service;
using Microsoft.AspNetCore.Mvc;

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

    [HttpGet("current-user")]
    public async Task<IActionResult> GetCurrentUser()
    {
        try
        {
            var accessToken = await auth2Service.GetAccessTokenAsync();
            var userData = await auth2Service.GetCurrentUserAsync(accessToken);
            return Ok(userData);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "An error occurred: " + ex.Message);
        }
    }
}
