using System.Security.Claims;
using API.Base;
using Application.DTOs.Auth;
using Application.DTOs.Users;
using Application.Interfaces.Services.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Auth;

public class AuthController : BaseApiController
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    // API đăng nhập, dùng để đăng nhập vào hệ thống, sau khi đăng nhập sẽ trả về token 
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<TokenResponse>> Login([FromBody] LoginRequest request, CancellationToken ct)
    {
        var response = await _authService.LoginAsync(request, ct);
        return Ok(response);
    }

    // API refresh token, dùng để lấy token mới khi token cũ hết hạn
    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<ActionResult<TokenResponse>> Refresh([FromBody] RefreshTokenRequest request, CancellationToken ct)
    {
        var response = await _authService.RefreshTokenAsync(request, ct);
        return Ok(response);
    }

    // API revoke token, dùng để thu hồi token, sau khi thu hồi token sẽ không thể sử dụng được nữa
    [HttpPost("revoke")]
    [Authorize]
    public async Task<ActionResult> Revoke([FromBody] RefreshTokenRequest request, CancellationToken ct)
    {
        await _authService.RevokeTokenAsync(request, ct);
        return NoContent();
    }

    // API lấy thông tin người dùng hiện tại, dùng để hiển thị thông tin người dùng trên giao diện
    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<UserDto>> GetMe(CancellationToken ct)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized();
        }

        var user = await _authService.GetMeAsync(userId, ct);
        return Ok(user);
    }
}
