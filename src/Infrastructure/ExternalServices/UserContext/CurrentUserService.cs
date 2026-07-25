using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Infrastructure;

[RegisterService(typeof(ICurrentUserService))]
public sealed class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    public Guid? UserId => TryGetGuidClaim(ClaimTypes.NameIdentifier);
    public Guid? AccountId => TryGetGuidClaim("AccountId");

    private Guid? TryGetGuidClaim(string claimType)
    {
        var value = httpContextAccessor.HttpContext?.User.FindFirst(claimType)?.Value;
        return Guid.TryParse(value, out var id) ? id : null;
    }
}
