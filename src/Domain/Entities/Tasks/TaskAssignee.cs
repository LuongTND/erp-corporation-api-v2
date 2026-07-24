namespace Domain;

public class TaskAssignee : EntityBase<Guid>
{
    public Guid TaskID { get; set; }
    public TaskItem? Task { get; set; }

    public Guid UserID { get; set; }
    public User? User { get; set; }

    public DateTimeOffset AssignedAt { get; set; }
    public Guid AssignedBy { get; set; }
    public bool IsPrimaryAssignee { get; set; }
}
