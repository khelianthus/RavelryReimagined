using Newtonsoft.Json.Linq;
using RavelryReimagined.API.Models;
using System.Text.Json;

namespace API.Service;

public interface IRavelryService
{
    //, string sort = "status", string include = "collections", int page = 1, int pageSize = 20
    Task<ProjectsData> ListProjects(string username, string accessToken);
}
