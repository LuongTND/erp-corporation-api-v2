namespace Domain;

public class TaskAssignee
{
    public Guid TaskID { get; private set; }
    public virtual TaskItem Task { get; private set; } = null!;

    public Guid UserID { get; private set; }
    public virtual User User { get; private set; } = null!;

    public DateTime AssignedAt { get; private set; }
    public Guid AssignedBy { get; private set; }
    public bool IsPrimaryAssignee { get; private set; }

    private TaskAssignee()
    {
    }

    public static TaskAssignee Create(Guid taskId, Guid userId, Guid assignedBy, bool isPrimaryAssignee = false)
    {
        return new TaskAssignee
        {
            TaskID = taskId,
            UserID = userId,
            AssignedAt = DateTime.UtcNow,
            AssignedBy = assignedBy,
            IsPrimaryAssignee = isPrimaryAssignee
        };
    }
}