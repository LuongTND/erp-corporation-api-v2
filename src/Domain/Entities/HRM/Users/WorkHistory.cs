namespace Domain;

public class WorkHistory : EntityBase<Guid>
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public WorkHistoryChangeType ChangeType { get; set; }
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
    public string? Note { get; set; }

    public Guid? ChangedBy { get; set; }
    public DateTimeOffset ChangedAt { get; set; }
}
