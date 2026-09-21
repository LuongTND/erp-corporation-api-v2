namespace Application;

public sealed class CreateRoundTypeCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<CreateRoundTypeCommand, Guid>
{
    public async Task<Guid> Handle(CreateRoundTypeCommand cmd, CancellationToken ct)
    {
        var exists = await unitOfWork.Repository<Domain.RoundType>()
            .AnyAsync(r => r.Name == cmd.Name, ct);
        if (exists)
            throw new ConflictException("Loại vòng với tên này đã tồn tại.");

        var roundType = new Domain.RoundType
        {
            Id = Guid.NewGuid(),
            Name = cmd.Name,
            DisplayOrder = cmd.DisplayOrder,
            IsSystem = false
        };

        await unitOfWork.Repository<Domain.RoundType>().AddAsync(roundType);
        await unitOfWork.EnsureSaveAsync(ct);
        return roundType.Id;
    }
}
