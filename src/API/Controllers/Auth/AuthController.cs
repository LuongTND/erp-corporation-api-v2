namespace API;

public class AuthController : BaseApiController
{
    private readonly IAuthService _authService;
    private readonly IUserAccountRepository _userAccountRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AuthController(IAuthService authService, IUserAccountRepository userAccountRepository, IUnitOfWork unitOfWork)
    {
        _authService = authService;
        _userAccountRepository = userAccountRepository;
        _unitOfWork = unitOfWork;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<TokenResponse>> Login([FromBody] LoginRequest request, CancellationToken ct)
    {
        var response = await _authService.LoginAsync(request, ct);
        return Ok(response);
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<ActionResult<TokenResponse>> Refresh([FromBody] RefreshTokenRequest request, CancellationToken ct)
    {
        var response = await _authService.RefreshTokenAsync(request, ct);
        return Ok(response);
    }

    [HttpPost("revoke")]
    [Authorize]
    public async Task<ActionResult> Revoke([FromBody] RefreshTokenRequest request, CancellationToken ct)
    {
        await _authService.RevokeTokenAsync(request, ct);
        return NoContent();
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<UserDto>> GetMe(CancellationToken ct)
    {
        var user = await _authService.GetCurrentUserAsync(ct);
        return Ok(user);
    }

    // ponytail: dev-only, remove before prod
    [HttpPost("dev-reset-password")]
    [AllowAnonymous]
    public async Task<ActionResult> DevResetPassword([FromBody] DevResetPasswordRequest request, CancellationToken ct)
    {
        var account = await _userAccountRepository.GetByEmailWithUserAsync(request.Email.ToLowerInvariant(), ct);
        if (account == null) return NotFound("Email không tồn tại.");
        account.UpdatePassword(PasswordHasher.Hash(request.NewPassword));
        await _unitOfWork.SaveChangesAsync(ct);
        return Ok("Đổi mật khẩu thành công.");
    }
}

public record DevResetPasswordRequest(string Email, string NewPassword);