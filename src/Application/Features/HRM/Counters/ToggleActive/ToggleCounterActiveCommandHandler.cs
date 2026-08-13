namespace Application;

public sealed class ToggleCounterActiveCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<ToggleCounterActiveCommand, bool>
{
    public async Task<bool> Handle(ToggleCounterActiveCommand cmd, CancellationToken ct)
    {
        var counter = await unitOfWork.Repository<Counter>()
            .FindTrackedAsync(c => c.Id == cmd.CounterId && !c.IsDeleted, ct)
            ?? throw new NotFoundException(ExceptionMessages.NotFound("Counter", cmd.CounterId));

        counter.IsActive = !counter.IsActive;
        await unitOfWork.EnsureSaveAsync(ct);
        return counter.IsActive;
    }
}
