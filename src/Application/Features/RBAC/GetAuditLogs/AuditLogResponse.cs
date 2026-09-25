namespace Application;

public sealed record AuditLogResponse
{
    public long Id { get; init; }
    public string TableName { get; init; } = default!;
    public string EntityId { get; init; } = default!;
    public string Action { get; init; } = default!;
    public string? FieldName { get; init; }
    public string? OldValue { get; init; }
    public string? NewValue { get; init; }
    public Guid? UserId { get; init; }
    public DateTimeOffset Timestamp { get; init; }
}
