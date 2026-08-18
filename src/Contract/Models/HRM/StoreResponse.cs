namespace Contract;

public sealed class StoreResponse
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Code { get; init; } = string.Empty;
    public string PosStoreId { get; init; } = string.Empty;
    public string? Address { get; init; }
    public string? Phone { get; init; }
    public Guid? RegionId { get; init; }
    public Guid? ManagerId { get; init; }
    public string? ManagerName { get; init; }
    public bool IsActive { get; init; }
    public bool? TodayIsClosed { get; init; }
}
