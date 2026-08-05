namespace Application;

public sealed class SetScopeOverrideCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<SetScopeOverrideCommand, Unit>
{
    public async Task<Unit> Handle(SetScopeOverrideCommand cmd, CancellationToken ct)
    {
        var user = await unitOfWork.Repository<User>()
            .FindTrackedAsync(u => u.Id == cmd.UserId, ct)
            ?? throw new NotFoundException(ExceptionMessages.NotFound("User", cmd.UserId));

        user.ScopeOverride = cmd.ScopeOverride;

        await unitOfWork.EnsureSaveAsync(ct);
        return Unit.Value;
    }
}
