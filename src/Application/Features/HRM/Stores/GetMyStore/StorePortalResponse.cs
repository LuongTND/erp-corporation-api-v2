namespace Application;

public sealed record StorePortalResponse
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Code { get; init; } = string.Empty;
    public string? Address { get; init; }
    public string? Phone { get; init; }
    public string? RegionName { get; init; }
    public bool IsActive { get; init; }
    public StoreHoursResponse? TodayHours { get; init; }
    public IEnumerable<CounterResponse> Counters { get; init; } = [];
}
