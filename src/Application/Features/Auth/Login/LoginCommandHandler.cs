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
        var account = await unitOfWork.Repository<UserAccount>().Query(tracking: true)
            .Include(a => a.User)
                .ThenInclude(u => u!.UserRoles)
                    .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(a => a.LoginEmail == cmd.Email, ct)
            ?? throw new UnauthorizedException("Email hoặc mật khẩu không đúng.");

        if (account.IsLocked)
            throw new UnauthorizedException("Tài khoản đã bị tạm khóa. Vui lòng liên hệ quản trị viên.");

        if (!passwordHasher.Verify(cmd.Password, account.PasswordHash ?? string.Empty))
        {
            account.RecordLoginFailure();
            await unitOfWork.EnsureSaveAsync(ct);
            throw new UnauthorizedException("Email hoặc mật khẩu không đúng.");
        }

        if (account.User is null || !account.User.IsActive)
            throw new UnauthorizedException("Tài khoản không hoạt động.");

        var jwtOptions = appConfiguration.GetJwtOptions();
        var refreshToken = account.SetRefreshToken(jwtOptions.RefreshTokenLifetime);
        account.RecordLoginSuccess();
        await unitOfWork.EnsureSaveAsync(ct);

        return jwtTokensService.GenerateAccessToken(mapper.Map<UserCredentials>(account.User), refreshToken);
    }
}
