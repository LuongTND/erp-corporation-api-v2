namespace Application;

public sealed class DeleteLabelCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<DeleteLabelCommand, Unit>
{
    public async Task<Unit> Handle(DeleteLabelCommand cmd, CancellationToken ct)
    {
        var label = await unitOfWork.Repository<Label>()
            .FindTrackedAsync(l => l.Id == cmd.LabelId, ct)
            ?? throw new NotFoundException(ExceptionMessages.NotFound("Label", cmd.LabelId));

        await unitOfWork.Repository<Label>().RemoveAsync(label);
        await unitOfWork.EnsureSaveAsync(ct);
        return Unit.Value;
    }
}
