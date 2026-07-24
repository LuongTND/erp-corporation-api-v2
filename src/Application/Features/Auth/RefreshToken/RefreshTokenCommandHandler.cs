namespace Application;

public sealed class RefreshTokenCommandHandler(
    IUnitOfWork unitOfWork,
    IJwtTokensService jwtTokensService,
    IAppConfiguration appConfiguration,
    IMapper mapper)
    : IRequestHandler<RefreshTokenCommand, SignInResponse>
{
    public async Task<SignInResponse> Handle(RefreshTokenCommand cmd, CancellationToken ct)
    {
        var account = await unitOfWork.Repository<UserAccount>()
            .FindTrackedAsync(a => a.RefreshToken == cmd.RefreshToken, ct)
            ?? throw new UnauthorizedException("Refresh token không hợp lệ hoặc đã hết hạn.");

        if (!account.IsRefreshTokenValid(cmd.RefreshToken))
            throw new UnauthorizedException("Refresh token không hợp lệ hoặc đã hết hạn.");

        var user = await unitOfWork.Repository<User>()
            .FindAsync(u => u.Id == account.UserId && u.IsActive, ct, u => u.UserRoles)
            ?? throw new UnauthorizedException("Tài khoản không hoạt động.");

        var jwtOptions = appConfiguration.GetJwtOptions();
        var newRefreshToken = account.SetRefreshToken(jwtOptions.RefreshTokenLifetime);
        await unitOfWork.EnsureSaveAsync(ct);

        return jwtTokensService.GenerateAccessToken(mapper.Map<UserCredentials>(user), newRefreshToken);
    }
}
