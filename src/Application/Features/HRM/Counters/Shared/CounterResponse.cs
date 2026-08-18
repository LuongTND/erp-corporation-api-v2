namespace Application;

public sealed record CounterResponse
{
    public Guid Id { get; init; }
    public Guid StoreId { get; init; }
    public string StoreName { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string Code { get; init; } = string.Empty;
    public bool IsActive { get; init; }
}
