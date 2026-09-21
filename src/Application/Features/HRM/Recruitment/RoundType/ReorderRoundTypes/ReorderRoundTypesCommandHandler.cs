namespace Application;

public sealed class ReorderRoundTypesCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<ReorderRoundTypesCommand, Unit>
{
    public async Task<Unit> Handle(ReorderRoundTypesCommand cmd, CancellationToken ct)
    {
        var ids = cmd.Items.Select(i => i.Id).ToList();
        var roundTypes = await unitOfWork.Repository<Domain.RoundType>()
            .GetAllTrackedAsync(r => ids.Contains(r.Id), ct);

        foreach (var roundType in roundTypes)
        {
            var item = cmd.Items.First(i => i.Id == roundType.Id);
            roundType.DisplayOrder = item.DisplayOrder;
        }

        await unitOfWork.SaveChangesAsync(ct);
        return Unit.Value;
    }
}
