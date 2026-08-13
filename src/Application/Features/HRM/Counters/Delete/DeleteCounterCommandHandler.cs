namespace Application;

public sealed class DeleteCounterCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<DeleteCounterCommand, Unit>
{
    public async Task<Unit> Handle(DeleteCounterCommand cmd, CancellationToken ct)
    {
        var counter = await unitOfWork.Repository<Counter>()
            .FindTrackedAsync(c => c.Id == cmd.CounterId && !c.IsDeleted, ct)
            ?? throw new NotFoundException(ExceptionMessages.NotFound("Counter", cmd.CounterId));

        counter.IsDeleted = true;
        counter.DeletedAt = DateTimeOffset.UtcNow;
        await unitOfWork.EnsureSaveAsync(ct);
        return Unit.Value;
    }
}
