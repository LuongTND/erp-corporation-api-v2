namespace Application;

public sealed class CreateCounterCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<CreateCounterCommand, Guid>
{
    public async Task<Guid> Handle(CreateCounterCommand cmd, CancellationToken ct)
    {
        var storeExists = await unitOfWork.Repository<Store>()
            .AnyAsync(s => s.Id == cmd.StoreId && !s.IsDeleted, ct);
        if (!storeExists) throw new NotFoundException(ExceptionMessages.NotFound("Store", cmd.StoreId));

        var duplicate = await unitOfWork.Repository<Counter>()
            .AnyAsync(c => c.StoreId == cmd.StoreId && c.Code == cmd.Code && !c.IsDeleted, ct);
        if (duplicate) throw new ConflictException($"Mã quầy '{cmd.Code}' đã tồn tại trong cửa hàng này.");

        var counter = new Counter
        {
            Id = Guid.NewGuid(),
            StoreId = cmd.StoreId,
            Name = cmd.Name,
            Code = cmd.Code,
            IsActive = true,
        };

        await unitOfWork.Repository<Counter>().AddAsync(counter);
        await unitOfWork.EnsureSaveAsync(ct);
        return counter.Id;
    }
}
