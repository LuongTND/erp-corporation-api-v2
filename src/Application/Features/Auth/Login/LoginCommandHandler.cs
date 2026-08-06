namespace Application;

public sealed class LoginCommandHandler(
    IUnitOfWork unitOfWork,
    IPasswordHasher passwordHasher,
    IJwtTokensService jwtTokensService,
    IAppConfiguration appConfiguration,
    IMapper mapper)
    : IRequestHandler<LoginCommand, SignInResponse>
{
    public async Task<SignInResponse> Handle(LoginCommand cmd, CancellationToken ct)
    {
        var account = await unitOfWork.Repository<UserAccount>()
            .FindTrackedAsync(a => a.LoginEmail == cmd.Email, ct)
            ?? throw new UnauthorizedException("Email hoặc mật khẩu không đúng.");

        if (!passwordHasher.Verify(cmd.Password, account.PasswordHash ?? string.Empty))
            throw new UnauthorizedException("Email hoặc mật khẩu không đúng.");

        var user = await unitOfWork.Repository<User>()
            .FindAsync(u => u.Id == account.UserId && u.IsActive, ct, u => u.UserRoles)
            ?? throw new UnauthorizedException("Tài khoản không hoạt động.");

        // Load Role nav for each active UserRole (FindAsync doesn't support ThenInclude)
        foreach (var ur in user.UserRoles.Where(ur => ur.IsActive))
            ur.Role = await unitOfWork.Repository<Role>().FindAsync(r => r.Id == ur.RoleId, ct);

        var jwtOptions = appConfiguration.GetJwtOptions();
        var refreshToken = account.SetRefreshToken(jwtOptions.RefreshTokenLifetime);
        account.RecordLoginSuccess();
        await unitOfWork.EnsureSaveAsync(ct);

        return jwtTokensService.GenerateAccessToken(mapper.Map<UserCredentials>(user), refreshToken);
    }
}
