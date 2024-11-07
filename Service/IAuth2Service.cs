namespace API.Service;

public interface IAuth2Service
{
    Task<string> GetAccessTokenAsync();
    Task<string> GetCurrentUserAsync(string accessToken);
}
