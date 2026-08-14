namespace Application;

public sealed class RemoveStoreMemberCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<RemoveStoreMemberCommand, Unit>
{
    public async Task<Unit> Handle(RemoveStoreMemberCommand cmd, CancellationToken ct)
    {
        var membership = await unitOfWork.Repository<UserStore>()
            .FindTrackedAsync(us => us.UserId == cmd.UserId && us.StoreId == cmd.StoreId && us.IsActive, ct)
            ?? throw new NotFoundException("Nhân viên không thuộc cửa hàng này.");

        membership.IsActive = false;
        membership.EndDate = DateOnly.FromDateTime(DateTime.UtcNow.Date);
        await unitOfWork.EnsureSaveAsync(ct);
        return Unit.Value;
    }
}
