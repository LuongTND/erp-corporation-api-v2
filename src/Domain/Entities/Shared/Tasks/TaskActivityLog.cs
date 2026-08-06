namespace Domain;

public class TaskActivityLog : EntityBase<Guid>
{
    public Guid TaskID { get; set; }
    public TaskItem? Task { get; set; }

    public Guid UserID { get; set; }
    public User? User { get; set; }

    public TaskActivityAction Action { get; set; }
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
}
