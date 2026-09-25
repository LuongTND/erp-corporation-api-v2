namespace Application;

public sealed class LockEmployeeCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<LockEmployeeCommand, Unit>
{
    public async Task<Unit> Handle(LockEmployeeCommand cmd, CancellationToken ct)
    {
        var account = await unitOfWork.Repository<UserAccount>()
            .FindTrackedAsync(a => a.UserId == cmd.UserId, ct)
            ?? throw new NotFoundException($"UserAccount for user {cmd.UserId} not found");

        account.IsLocked = cmd.Lock;
        await unitOfWork.SaveChangesAsync(ct);
        return Unit.Value;
    }
}
