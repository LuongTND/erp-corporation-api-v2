namespace Application;

public sealed record GetRegionHoursQuery(Guid RegionId) : IRequest<IEnumerable<RegionHoursResponse>>;
