using System.IdentityModel.Tokens.Jwt;

namespace API;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal principal)
    {
        var value = principal.FindFirstValue(JwtRegisteredClaimNames.Sub);

        if (string.IsNullOrEmpty(value) || !Guid.TryParse(value, out var id))
            throw new UnauthorizedException("Invalid or missing user ID in token");

        return id;
    }
}
