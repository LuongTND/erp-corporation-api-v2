namespace Application;

public sealed class ToggleStoreActiveCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<ToggleStoreActiveCommand, bool>
{
    public async Task<bool> Handle(ToggleStoreActiveCommand cmd, CancellationToken ct)
    {
        var store = await unitOfWork.Repository<Store>()
            .FindTrackedAsync(s => s.Id == cmd.StoreId && !s.IsDeleted, ct)
            ?? throw new NotFoundException(ExceptionMessages.NotFound("Store", cmd.StoreId));

        store.IsActive = !store.IsActive;
        await unitOfWork.EnsureSaveAsync(ct);
        return store.IsActive;
    }
}
