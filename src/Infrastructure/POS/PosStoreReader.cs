namespace Infrastructure;

[RegisterService(typeof(IPosStoreReader))]
public sealed class PosStoreReader(PosReadDbContext posDb) : IPosStoreReader
{
    public async Task<IEnumerable<PosStoreResponse>> GetAllStoresAsync(CancellationToken ct = default)
    {
        var rows = await posDb.Stores
            .GroupJoin(posDb.Regions,
                s => s.RegionId,
                r => r.Id,
                (s, regions) => new { s, regions })
            .SelectMany(
                x => x.regions.DefaultIfEmpty(),
                (x, r) => new { x.s.Id, x.s.Name, x.s.Address, x.s.Phone, x.s.RegionId, RegionName = r != null ? r.Name : null })
            .OrderBy(s => s.Name)
            .ToListAsync(ct);
        return rows.Select(r => new PosStoreResponse(r.Id, r.Name, r.Address, r.Phone, r.RegionId, r.RegionName));
    }

    public async Task<PosStoreResponse?> FindStoreAsync(Guid posStoreId, CancellationToken ct = default)
    {
        var row = await posDb.Stores
            .Where(s => s.Id == posStoreId)
            .GroupJoin(posDb.Regions,
                s => s.RegionId,
                r => r.Id,
                (s, regions) => new { s, regions })
            .SelectMany(
                x => x.regions.DefaultIfEmpty(),
                (x, r) => new { x.s.Id, x.s.Name, x.s.Address, x.s.Phone, x.s.RegionId, RegionName = r != null ? r.Name : null })
            .FirstOrDefaultAsync(ct);
        return row is null ? null : new PosStoreResponse(row.Id, row.Name, row.Address, row.Phone, row.RegionId, row.RegionName);
    }
}
