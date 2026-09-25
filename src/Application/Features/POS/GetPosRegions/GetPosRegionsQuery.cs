namespace Application;

public sealed record GetPosRegionsQuery : IRequest<IEnumerable<PosRegionResponse>>;

public sealed record PosRegionResponse(Guid Id, string Code, string Name, bool IsActive);
