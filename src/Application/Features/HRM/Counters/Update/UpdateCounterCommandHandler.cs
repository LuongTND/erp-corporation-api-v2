namespace Application;

public sealed class UpdateCounterCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateCounterCommand, Unit>
{
    public async Task<Unit> Handle(UpdateCounterCommand cmd, CancellationToken ct)
    {
        var counter = await unitOfWork.Repository<Counter>()
            .FindTrackedAsync(c => c.Id == cmd.CounterId && !c.IsDeleted, ct)
            ?? throw new NotFoundException(ExceptionMessages.NotFound("Counter", cmd.CounterId));

        var duplicate = await unitOfWork.Repository<Counter>()
            .AnyAsync(c => c.StoreId == counter.StoreId && c.Code == cmd.Code && c.Id != cmd.CounterId && !c.IsDeleted, ct);
        if (duplicate) throw new ConflictException($"Mã quầy '{cmd.Code}' đã tồn tại trong cửa hàng này.");

        counter.Name = cmd.Name;
        counter.Code = cmd.Code;

        await unitOfWork.EnsureSaveAsync(ct);
        return Unit.Value;
    }
}
