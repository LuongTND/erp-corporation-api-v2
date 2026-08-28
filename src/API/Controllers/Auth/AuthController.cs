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
    [HttpGet("me/permissions")]
    public async Task<ActionResult<ApiResponse<IReadOnlyCollection<string>>>> GetMyPermissions(CancellationToken ct)
        => Ok(ApiResponse<IReadOnlyCollection<string>>.Ok(await sender.Send(new GetMyPermissionsQuery(), ct)));

    [Authorize]
    [HttpPut("change-password")]
    public async Task<ActionResult<ApiResponse<Unit>>> ChangePassword(
        ChangePasswordCommand cmd, CancellationToken ct)
        => Ok(ApiResponse<Unit>.Ok(await sender.Send(cmd, ct)));

    // Nhân viên tự xem hồ sơ đầy đủ của mình — khác GET /api/auth/me (chỉ trả basic info).
    // Tái dùng GetUserDetailQuery với UserId = caller, bỏ qua data-scope check.
    [Authorize]
    [HttpGet("me/detail")]
    public async Task<ActionResult<ApiResponse<UserDetailResponse>>> MyDetail(
        [FromServices] IUserContext userContext, CancellationToken ct)
        => Ok(ApiResponse<UserDetailResponse>.Ok(
            await sender.Send(new GetUserDetailQuery(userContext.UserId, userContext.UserId), ct)));

    // Nhân viên xem lương hiện tại của mình — khác GET /api/hr/users/{id}/salary/current (admin only).
    [Authorize]
    [HttpGet("me/salary")]
    public async Task<ActionResult<ApiResponse<SalaryRecordResponse?>>> MySalary(
        [FromServices] IUserContext userContext, CancellationToken ct)
        => Ok(ApiResponse<SalaryRecordResponse?>.Ok(
            await sender.Send(new GetCurrentSalaryQuery(userContext.UserId), ct)));

    // Nhân viên tự cập nhật thông tin cá nhân, giấy tờ, tài chính — không cho sửa FullName/JobLevel/Manager (HR quản lý).
    [Authorize]
    [HttpPatch("me/profile")]
    public async Task<ActionResult<ApiResponse<Unit>>> UpdateMyProfile(
        UpdateMyProfileCommand cmd, CancellationToken ct)
        => Ok(ApiResponse<Unit>.Ok(await sender.Send(cmd, ct)));
}