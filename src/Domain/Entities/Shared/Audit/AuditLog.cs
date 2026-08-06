namespace Domain;

public class AuditLog
{
    public long Id { get; set; }
    public string TableName { get; set; } = default!;
    public string EntityId { get; set; } = default!;
    public string Action { get; set; } = default!;   // Created | Updated | Deleted
    public string? FieldName { get; set; }
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
    public Guid? UserId { get; set; }
    public DateTimeOffset Timestamp { get; set; }
}
