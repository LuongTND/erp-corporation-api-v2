namespace Application;

public sealed class GetPosRegionsQueryHandler(IPosRegionReader posReader)
    : IRequestHandler<GetPosRegionsQuery, IEnumerable<PosRegionResponse>>
{
    public Task<IEnumerable<PosRegionResponse>> Handle(GetPosRegionsQuery query, CancellationToken ct)
        => posReader.GetAllRegionsAsync(ct);
}
