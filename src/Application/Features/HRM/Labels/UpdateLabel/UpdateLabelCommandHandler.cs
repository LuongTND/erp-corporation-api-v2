namespace Application;

public sealed class UpdateLabelCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateLabelCommand, Unit>
{
    public async Task<Unit> Handle(UpdateLabelCommand cmd, CancellationToken ct)
    {
        var label = await unitOfWork.Repository<Label>()
            .FindTrackedAsync(l => l.Id == cmd.LabelId, ct)
            ?? throw new NotFoundException(ExceptionMessages.NotFound("Label", cmd.LabelId));

        var nameTaken = await unitOfWork.Repository<Label>()
            .AnyAsync(l => l.Name == cmd.Name && l.Id != cmd.LabelId, ct);
        if (nameTaken) throw new ConflictException($"Nhãn '{cmd.Name}' đã tồn tại");

        label.Name = cmd.Name;
        label.Color = cmd.Color;
        label.IsActive = cmd.IsActive;
        await unitOfWork.EnsureSaveAsync(ct);
        return Unit.Value;
    }
}
