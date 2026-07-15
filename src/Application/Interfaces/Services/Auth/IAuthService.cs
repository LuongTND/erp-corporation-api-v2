
namespace Application;
public interface IAuthService
{
    Task<TokenResponse> LoginAsync(LoginRequest request, CancellationToken ct = default);
    Task<TokenResponse> RefreshTokenAsync(RefreshTokenRequest request, CancellationToken ct = default);
    Task RevokeTokenAsync(RefreshTokenRequest request, CancellationToken ct = default);
    Task<UserDto> GetMeAsync(Guid userId, CancellationToken ct = default);
    Task<UserDto> GetCurrentUserAsync(CancellationToken ct = default);
}
