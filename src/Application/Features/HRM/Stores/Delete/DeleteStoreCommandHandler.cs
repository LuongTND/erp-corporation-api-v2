namespace Application;

public sealed class DeleteStoreCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<DeleteStoreCommand, Unit>
{
    public async Task<Unit> Handle(DeleteStoreCommand cmd, CancellationToken ct)
    {
        var store = await unitOfWork.Repository<Store>()
            .FindTrackedAsync(s => s.Id == cmd.StoreId, ct)
            ?? throw new NotFoundException(ExceptionMessages.NotFound("Store", cmd.StoreId));

        store.IsDeleted = true;
        store.DeletedAt = DateTimeOffset.UtcNow;
        store.IsActive = false;

        await unitOfWork.EnsureSaveAsync(ct);
        return Unit.Value;
    }
}
