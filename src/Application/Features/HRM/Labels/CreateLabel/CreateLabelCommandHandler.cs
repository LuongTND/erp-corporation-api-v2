namespace Application;

public sealed class CreateLabelCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<CreateLabelCommand, Guid>
{
    public async Task<Guid> Handle(CreateLabelCommand cmd, CancellationToken ct)
    {
        var exists = await unitOfWork.Repository<Label>().AnyAsync(l => l.Name == cmd.Name, ct);
        if (exists) throw new ConflictException($"Nhãn '{cmd.Name}' đã tồn tại");

        var label = new Label { Id = Guid.NewGuid(), Name = cmd.Name, Color = cmd.Color };
        await unitOfWork.Repository<Label>().AddAsync(label);
        await unitOfWork.EnsureSaveAsync(ct);
        return label.Id;
    }
}
