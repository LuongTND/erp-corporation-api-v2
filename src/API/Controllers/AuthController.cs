namespace API;

[ApiController]
[Route("api/auth")]
public sealed class AuthController(ISender sender) : ControllerBase
{
    [HttpPost("login")]
    public async Task<ActionResult<ApiResponse<SignInResponse>>> Login(
        LoginCommand cmd, CancellationToken ct)
        => Ok(ApiResponse<SignInResponse>.Ok(await sender.Send(cmd, ct)));

    [HttpPost("refresh")]
    public async Task<ActionResult<ApiResponse<SignInResponse>>> Refresh(
        RefreshTokenCommand cmd, CancellationToken ct)
        => Ok(ApiResponse<SignInResponse>.Ok(await sender.Send(cmd, ct)));

    [Authorize]
    [HttpPost("logout")]
    public async Task<ActionResult<ApiResponse<Unit>>> Logout(CancellationToken ct)
        => Ok(ApiResponse<Unit>.Ok(await sender.Send(new LogoutCommand(), ct)));

    [Authorize]
    [HttpGet("me")]
    public async Task<ActionResult<ApiResponse<UserProfileResponse>>> Me(CancellationToken ct)
        => Ok(ApiResponse<UserProfileResponse>.Ok(await sender.Send(new GetProfileQuery(), ct)));

    [Authorize]
    [HttpPut("me")]
    public async Task<ActionResult<ApiResponse<UserProfileResponse>>> UpdateProfile(
        UpdateProfileCommand cmd, CancellationToken ct)
        => Ok(ApiResponse<UserProfileResponse>.Ok(await sender.Send(cmd, ct)));

    [Authorize]
    [HttpPut("change-password")]
    public async Task<ActionResult<ApiResponse<Unit>>> ChangePassword(
        ChangePasswordCommand cmd, CancellationToken ct)
        => Ok(ApiResponse<Unit>.Ok(await sender.Send(cmd, ct)));
}