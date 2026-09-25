namespace Application;

public sealed class GetPosStoresQueryHandler(IPosStoreReader posReader)
    : IRequestHandler<GetPosStoresQuery, IEnumerable<PosStoreResponse>>
{
    public async Task<IEnumerable<PosStoreResponse>> Handle(GetPosStoresQuery _, CancellationToken ct)
        => await posReader.GetAllStoresAsync(ct);
}
