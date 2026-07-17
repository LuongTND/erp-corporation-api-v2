
namespace Domain;
public class TaskActivityLog : BaseEntity
{
    public Guid TaskID { get; private set; }
    public virtual TaskItem Task { get; private set; } = null!;

    public Guid UserID { get; private set; }
    public virtual User User { get; private set; } = null!;

    public TaskActivityAction Action { get; private set; }
    public string? OldValue { get; private set; }
    public string? NewValue { get; private set; }

    private TaskActivityLog() : base()
    {
    }

    public static TaskActivityLog Create(Guid taskId, Guid userId, TaskActivityAction action, string? oldValue = null, string? newValue = null)
    {
        return new TaskActivityLog
        {
            TaskID = taskId,
            UserID = userId,
            Action = action,
            OldValue = oldValue,
            NewValue = newValue
        };
    }
}
