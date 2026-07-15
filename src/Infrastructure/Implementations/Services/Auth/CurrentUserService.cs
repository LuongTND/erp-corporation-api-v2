using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Infrastructure;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid? UserId
    {
        get
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(userIdClaim, out var userId) ? userId : null;
        }
    }

    public Guid? AccountId
    {
        get
        {
            var accountIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst("AccountId")?.Value;
            return Guid.TryParse(accountIdClaim, out var accountId) ? accountId : null;
        }
    }
}