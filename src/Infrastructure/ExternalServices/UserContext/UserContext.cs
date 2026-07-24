using Microsoft.AspNetCore.Http;

namespace Infrastructure;

[RegisterService(typeof(IUserContext))]
public class UserContext(IHttpContextAccessor httpContextAccessor) : IUserContext
{
    private const string UserIdClaim = "user_id";

    public Guid UserId => GetGuidClaim(UserIdClaim);

    private Guid GetGuidClaim(string claimType)
    {
        var value = httpContextAccessor.HttpContext?.User.FindFirst(claimType)?.Value;

        if (string.IsNullOrEmpty(value) || !Guid.TryParse(value, out var id))
            throw new UnauthorizedException($"Invalid or missing '{claimType}' in token");

        return id;
    }
}
