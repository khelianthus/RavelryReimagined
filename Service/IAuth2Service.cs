using System.Text.Json;

namespace API.Service;

public interface IAuth2Service
{
    Task<string> GetCurrentUser(string accessToken);
}
