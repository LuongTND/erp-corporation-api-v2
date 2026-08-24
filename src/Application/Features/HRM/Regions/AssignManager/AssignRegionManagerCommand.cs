namespace Application;

public sealed record AssignRegionManagerCommand(Guid RegionId, Guid? ManagerId) : IRequest<Unit>;
