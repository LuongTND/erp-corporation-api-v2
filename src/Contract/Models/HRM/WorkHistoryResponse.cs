namespace Contract;

public sealed class WorkHistoryResponse
{
    public Guid Id { get; init; }
    public string ChangeType { get; init; } = string.Empty;
    public string ChangeTypeLabel { get; init; } = string.Empty;
    public string? OldValue { get; init; }
    public string? NewValue { get; init; }
    public string? Note { get; init; }
    public Guid? ChangedBy { get; init; }
    public DateTimeOffset ChangedAt { get; init; }
}
