using Microsoft.AspNetCore.Http;
using System.Security.Claims;

public class UserContextService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserContextService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? GetCurrentUserId()
    {
        var identity = _httpContextAccessor.HttpContext?.User.Identities.FirstOrDefault(i => i.AuthenticationType == "user_id_data");

        if (identity != null)
        {
            var userID = identity.FindFirst(ClaimTypes.Sid)?.Value;
            return userID;
        }
        return null;

        // return _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Sid);
    }
}