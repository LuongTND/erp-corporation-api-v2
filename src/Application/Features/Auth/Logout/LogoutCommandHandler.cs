namespace Application;

public sealed class LogoutCommandHandler(
    IUserContext userContext,
    IUnitOfWork unitOfWork)
    : IRequestHandler<LogoutCommand, MediatR.Unit>
{
    public async Task<MediatR.Unit> Handle(LogoutCommand cmd, CancellationToken ct)
    {
        var account = await unitOfWork.Repository<UserAccount>()
            .FindTrackedAsync(a => a.UserId == userContext.UserId, ct);

        if (account is not null)
        {
            account.ClearRefreshToken();
            await unitOfWork.SaveChangesAsync(ct);
        }

        return MediatR.Unit.Value;
    }
}
