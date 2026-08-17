namespace Contract;

public sealed class RegionResponse
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Code { get; init; } = string.Empty;
    public string PosRegionId { get; init; } = string.Empty;
    public bool IsActive { get; init; }
    public int StoreCount { get; init; }
}
