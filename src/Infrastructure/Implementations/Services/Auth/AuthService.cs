using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure;

[RegisterService(typeof(IAuthService))]
public class AuthService : IAuthService
{
    private readonly IUserAccountRepository _userAccountRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IConfiguration _configuration;
    private readonly TimeProvider _timeProvider;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;
    private readonly IRoleRepository _roleRepository;

    public AuthService(
        IUserAccountRepository userAccountRepository,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        IConfiguration configuration,
        TimeProvider timeProvider,
        ICurrentUserService currentUserService,
        IMapper mapper,
        IRoleRepository roleRepository)
    {
        _userAccountRepository = userAccountRepository;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _configuration = configuration;
        _timeProvider = timeProvider;
        _currentUserService = currentUserService;
        _mapper = mapper;
        _roleRepository = roleRepository;
    }

    public async Task<TokenResponse> LoginAsync(LoginRequest request, CancellationToken ct = default)
    {
        var loginEmail = request.Email.ToLowerInvariant();
        var account = await _userAccountRepository.GetByEmailWithUserAsync(loginEmail, ct);

        if (account == null)
        {
            throw new NotFoundException("Tài khoản hoặc mật khẩu không chính xác.");
        }

        if (account.IsLocked)
        {
            throw new ConflictException("Tài khoản này đã bị khóa. Vui lòng liên hệ quản trị viên.");
        }

        if (string.IsNullOrEmpty(account.PasswordHash) ||
            !PasswordHasher.Verify(account.PasswordHash, request.Password))
        {
            account.RecordLoginFailure();
            await _unitOfWork.SaveChangesAsync(ct);
            throw new ConflictException("Tài khoản hoặc mật khẩu không chính xác.");
        }

        account.RecordLoginSuccess();

        var (tokenString, expiry) = GenerateAccessToken(account);
        var refreshToken = account.SetRefreshToken(GetRefreshTokenExpirationDays());

        await _unitOfWork.SaveChangesAsync(ct);

        return new TokenResponse(
            tokenString, expiry,
            account.User.EmployeeCode, account.User.FullName, account.UserId,
            refreshToken, account.RefreshTokenExpiresAt!.Value);
    }

    public async Task<TokenResponse> RefreshTokenAsync(RefreshTokenRequest request, CancellationToken ct = default)
    {
        var account = await _userAccountRepository.GetByRefreshTokenWithUserAsync(request.RefreshToken, ct);

        if (account == null || !account.IsRefreshTokenValid(request.RefreshToken))
        {
            throw new ConflictException("Refresh token không hợp lệ hoặc đã hết hạn.");
        }

        if (account.IsLocked)
        {
            throw new ConflictException("Tài khoản này đã bị khóa. Vui lòng liên hệ quản trị viên.");
        }

        var (tokenString, expiry) = GenerateAccessToken(account);
        var newRefreshToken = account.SetRefreshToken(GetRefreshTokenExpirationDays());

        await _unitOfWork.SaveChangesAsync(ct);

        return new TokenResponse(
            tokenString, expiry,
            account.User.EmployeeCode, account.User.FullName, account.UserId,
            newRefreshToken, account.RefreshTokenExpiresAt!.Value);
    }

    public async Task RevokeTokenAsync(RefreshTokenRequest request, CancellationToken ct = default)
    {
        var account = await _userAccountRepository.GetByRefreshTokenWithUserAsync(request.RefreshToken, ct);

        if (account != null)
        {
            account.ClearRefreshToken();
            await _unitOfWork.SaveChangesAsync(ct);
        }
    }

    public async Task<UserDto> GetCurrentUserAsync(CancellationToken ct = default)
    {
        var userId = _currentUserService.UserId
                     ?? throw new ForbiddenException("Không xác định được người dùng hiện tại.");

        return await GetMeAsync(userId, ct);
    }

    public async Task<UserDto> GetMeAsync(Guid userId, CancellationToken ct = default)
    {
        var user = await _userRepository.GetByIdWithDetailsAsync(userId, ct);

        if (user == null)
        {
            throw new NotFoundException("Không tìm thấy thông tin nhân sự.");
        }

        var dto = _mapper.Map<UserDto>(user);
        dto.BypassDataScope = await _roleRepository.HasBypassDataScopeRoleAsync(userId, ct);
        dto.Permissions = await _roleRepository.GetUserPermissionCodesAsync(userId, ct);
        return dto;
    }

    private (string tokenString, DateTime expiry) GenerateAccessToken(UserAccount account)
    {
        var key = _configuration["Jwt:SecretKey"] ??
                  _configuration["JWT_KEY"] ?? "SuperSecretKeyMustBeAtLeast32BytesLongForHmacSha256Signing!";
        var issuer = _configuration["Jwt:Issuer"] ?? _configuration["JWT_ISSUER"] ?? "erp-corporation-api";
        var audience = _configuration["Jwt:Audience"] ?? _configuration["JWT_AUDIENCE"] ?? "erp-corporation-app";
        var expiryMinutesStr = _configuration["Jwt:AccessTokenExpirationMinutes"] ??
                               _configuration["JWT_EXPIRY_MINUTES"] ?? "60";
        if (!double.TryParse(expiryMinutesStr, out var expiryMinutes) || expiryMinutes <= 0)
        {
            expiryMinutes = 60;
        }

        var tokenHandler = new JwtSecurityTokenHandler();
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var utcNow = _timeProvider.GetUtcNow().UtcDateTime;
        var expiry = utcNow.AddMinutes(expiryMinutes);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, account.UserId.ToString()),
            new Claim("AccountId", account.Id.ToString()),
            new Claim(ClaimTypes.Email, account.LoginEmail),
            new Claim(ClaimTypes.Name, account.User.FullName)
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = expiry,
            Issuer = issuer,
            Audience = audience,
            SigningCredentials = credentials
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return (tokenHandler.WriteToken(token), expiry);
    }

    private int GetRefreshTokenExpirationDays()
    {
        var daysStr = _configuration["Jwt:RefreshTokenExpirationDays"] ?? "30";
        return int.TryParse(daysStr, out var days) && days > 0 ? days : 30;
    }
}