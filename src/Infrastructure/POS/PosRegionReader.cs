namespace Infrastructure;

[RegisterService(typeof(IPosRegionReader))]
public sealed class PosRegionReader(PosReadDbContext posDb) : IPosRegionReader
{
    public async Task<IEnumerable<PosRegionResponse>> GetAllRegionsAsync(CancellationToken ct = default)
    {
        var rows = await posDb.Regions
            .OrderBy(r => r.Name)
            .ToListAsync(ct);
        return rows.Select(r => new PosRegionResponse(r.Id, r.Code, r.Name, r.IsActive));
    }
}
