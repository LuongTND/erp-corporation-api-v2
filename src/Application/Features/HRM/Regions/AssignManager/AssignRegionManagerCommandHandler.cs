namespace Application;

public sealed class AssignRegionManagerCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<AssignRegionManagerCommand, Unit>
{
    public async Task<Unit> Handle(AssignRegionManagerCommand cmd, CancellationToken ct)
    {
        var region = await unitOfWork.Repository<Region>()
            .FindTrackedAsync(r => r.Id == cmd.RegionId && !r.IsDeleted, ct)
            ?? throw new NotFoundException(ExceptionMessages.NotFound("Region", cmd.RegionId));

        if (cmd.ManagerId.HasValue)
        {
            var userExists = await unitOfWork.Repository<User>()
                .AnyAsync(u => u.Id == cmd.ManagerId.Value && u.IsActive, ct);
            if (!userExists)
                throw new NotFoundException(ExceptionMessages.NotFound("User", cmd.ManagerId.Value));
        }

        region.ManagerId = cmd.ManagerId;
        await unitOfWork.EnsureSaveAsync(ct);
        return Unit.Value;
    }
}
