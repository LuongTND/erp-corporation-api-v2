namespace Application;

public sealed class AddStoreMemberCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<AddStoreMemberCommand, Guid>
{
    public async Task<Guid> Handle(AddStoreMemberCommand cmd, CancellationToken ct)
    {
        var storeExists = await unitOfWork.Repository<Store>()
            .AnyAsync(s => s.Id == cmd.StoreId && !s.IsDeleted, ct);
        if (!storeExists)
            throw new NotFoundException(ExceptionMessages.NotFound("Store", cmd.StoreId));

        var userExists = await unitOfWork.Repository<User>()
            .AnyAsync(u => u.Id == cmd.UserId && u.IsActive, ct);
        if (!userExists)
            throw new NotFoundException(ExceptionMessages.NotFound("User", cmd.UserId));

        var duplicate = await unitOfWork.Repository<UserStore>()
            .AnyAsync(us => us.UserId == cmd.UserId && us.StoreId == cmd.StoreId && us.IsActive, ct);
        if (duplicate)
            throw new ConflictException("Nhân viên đã được gắn vào cửa hàng này.");

        if (cmd.IsHomeStore)
        {
            var existing = await unitOfWork.Repository<UserStore>()
                .GetAllTrackedAsync(us => us.UserId == cmd.UserId && us.IsHomeStore && us.IsActive, ct);
            foreach (var us in existing)
                us.IsHomeStore = false;
        }

        var entity = new UserStore
        {
            Id = Guid.NewGuid(),
            UserId = cmd.UserId,
            StoreId = cmd.StoreId,
            IsHomeStore = cmd.IsHomeStore,
            StartDate = cmd.StartDate,
            IsActive = true,
        };
        await unitOfWork.Repository<UserStore>().AddAsync(entity);
        await unitOfWork.EnsureSaveAsync(ct);
        return entity.Id;
    }
}
