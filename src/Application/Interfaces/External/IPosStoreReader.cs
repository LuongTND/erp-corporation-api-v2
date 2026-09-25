namespace Application;

public interface IPosStoreReader
{
    Task<IEnumerable<PosStoreResponse>> GetAllStoresAsync(CancellationToken ct = default);
    Task<PosStoreResponse?> FindStoreAsync(Guid posStoreId, CancellationToken ct = default);
}
