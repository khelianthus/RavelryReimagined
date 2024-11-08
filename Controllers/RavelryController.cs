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
    private readonly IRavelryService ravelryService;
    public RavelryController(IAuth2Service auth2Service, IRavelryService ravelryService)
    {
        this.auth2Service = auth2Service;
        this.ravelryService = ravelryService;
    }

    /// <summary>
    /// Get current-user.json
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

    /// <summary>
    /// Get all users projects 
    /// </summary>
    /// <param name="userName">The authenticated ravelry accounts username</param>
    /// <param name="accessToken">Authenticated accesstoken from token controller</param>
    /// <returns></returns>
    [HttpGet("projects")]
    public async Task<IActionResult> GetUserProjects(string userName, string accessToken)
    {
        try
        {
            var projectData = await ravelryService.ListProjects(userName, accessToken);

            if (projectData == null)
            {
                return NotFound("Projects not found.");
            }

            return Ok(projectData);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

}
