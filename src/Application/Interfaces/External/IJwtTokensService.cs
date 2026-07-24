using System.Security.Claims;

namespace Application;

public interface IJwtTokensService
{
    SignInResponse GenerateAccessToken(UserCredentials user, string refreshToken);
    string GenerateRefreshToken();
    ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    string GenerateEmailVerifyToken();
}
