namespace Application;

public sealed class UpdateRoundTypeCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateRoundTypeCommand, Unit>
{
    public async Task<Unit> Handle(UpdateRoundTypeCommand cmd, CancellationToken ct)
    {
        var roundType = await unitOfWork.Repository<Domain.RoundType>()
            .FindTrackedAsync(r => r.Id == cmd.Id, ct)
            ?? throw new NotFoundException("Loại vòng không tồn tại.");

        var duplicate = await unitOfWork.Repository<Domain.RoundType>()
            .AnyAsync(r => r.Name == cmd.Name && r.Id != cmd.Id, ct);
        if (duplicate)
            throw new ConflictException("Loại vòng với tên này đã tồn tại.");

        roundType.Name = cmd.Name;
        roundType.DisplayOrder = cmd.DisplayOrder;

        await unitOfWork.EnsureSaveAsync(ct);
        return Unit.Value;
    }
}
