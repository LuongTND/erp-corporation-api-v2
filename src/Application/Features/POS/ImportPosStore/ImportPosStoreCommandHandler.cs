namespace Application;

public sealed class ImportPosStoreCommandHandler(IUnitOfWork unitOfWork, IPosStoreReader posReader)
    : IRequestHandler<ImportPosStoreCommand, Guid>
{
    public async Task<Guid> Handle(ImportPosStoreCommand cmd, CancellationToken ct)
    {
        var posStore = await posReader.FindStoreAsync(cmd.PosStoreId, ct)
            ?? throw new NotFoundException(ExceptionMessages.NotFound("POS Store", cmd.PosStoreId));

        var alreadyImported = await unitOfWork.Repository<Store>()
            .AnyAsync(s => s.PosStoreId == cmd.PosStoreId.ToString(), ct);
        if (alreadyImported)
            throw new ConflictException($"Cửa hàng POS '{posStore.Name}' đã được import vào HRM rồi.");

        if (cmd.ManagerId.HasValue)
        {
            var manager = await unitOfWork.Repository<User>()
                .FindAsync(u => u.Id == cmd.ManagerId.Value, ct)
                ?? throw new NotFoundException(ExceptionMessages.NotFound("User (manager)", cmd.ManagerId.Value));

            if (!manager.IsActive)
                throw new BadRequestException("Trưởng cửa hàng đã bị vô hiệu hóa.");
        }

        var store = new Store
        {
            Id = Guid.NewGuid(),
            Name = posStore.Name,
            Code = $"STORE-{cmd.PosStoreId.ToString()[..8].ToUpperInvariant()}",
            PosStoreId = cmd.PosStoreId.ToString(),
            Address = posStore.Address,
            Phone = posStore.Phone,
            IsActive = true
        };

        await unitOfWork.Repository<Store>().AddAsync(store);
        await unitOfWork.EnsureSaveAsync(ct);
        return store.Id;
    }
}
