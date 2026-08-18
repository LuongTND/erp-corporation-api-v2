namespace Application;

public sealed record AuditLogFilter
{
    public string? TableName { get; init; }
    public string? Action { get; init; }
    public Guid? UserId { get; init; }
    public DateTimeOffset? From { get; init; }
    public DateTimeOffset? To { get; init; }
    public int Top { get; init; } = 50;
    public int Skip { get; init; } = 0;
    public bool NeedTotalCount { get; init; } = true;
}
