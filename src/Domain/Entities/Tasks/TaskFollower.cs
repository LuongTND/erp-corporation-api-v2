namespace Domain;

public class TaskFollower : EntityBase<Guid>
{
    public Guid TaskID { get; set; }
    public TaskItem? Task { get; set; }

    public Guid UserID { get; set; }
    public User? User { get; set; }

    public DateTimeOffset FollowedAt { get; set; }
}
