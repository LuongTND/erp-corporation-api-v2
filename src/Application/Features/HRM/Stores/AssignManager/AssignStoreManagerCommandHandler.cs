namespace Application;

public sealed class AssignStoreManagerCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<AssignStoreManagerCommand, Unit>
{
    public async Task<Unit> Handle(AssignStoreManagerCommand cmd, CancellationToken ct)
    {
        var store = await unitOfWork.Repository<Store>()
            .FindTrackedAsync(s => s.Id == cmd.StoreId && !s.IsDeleted, ct)
            ?? throw new NotFoundException(ExceptionMessages.NotFound("Store", cmd.StoreId));

        if (cmd.ManagerId.HasValue)
        {
            var userExists = await unitOfWork.Repository<User>()
                .AnyAsync(u => u.Id == cmd.ManagerId.Value && u.IsActive, ct);
            if (!userExists)
                throw new NotFoundException(ExceptionMessages.NotFound("User", cmd.ManagerId.Value));
        }

        store.ManagerId = cmd.ManagerId;
        await unitOfWork.EnsureSaveAsync(ct);
        return Unit.Value;
    }
}
