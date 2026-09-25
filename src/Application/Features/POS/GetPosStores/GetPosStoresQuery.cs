namespace Application;

public sealed record GetPosStoresQuery : IRequest<IEnumerable<PosStoreResponse>>;

public sealed record PosStoreResponse(
    Guid Id,
    string Name,
    string Address,
    string Phone,
    Guid? RegionId,
    string? RegionName);
