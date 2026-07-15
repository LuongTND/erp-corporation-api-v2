
namespace Domain;
public class TaskFollower
{
    public Guid TaskID { get; private set; }
    public virtual TaskItem Task { get; private set; } = null!;

    public Guid UserID { get; private set; }
    public virtual User User { get; private set; } = null!;

    public DateTime FollowedAt { get; private set; }

    private TaskFollower()
    {
    }

    public static TaskFollower Create(Guid taskId, Guid userId)
    {
        return new TaskFollower
        {
            TaskID = taskId,
            UserID = userId,
            FollowedAt = DateTime.UtcNow
        };
    }
}
