namespace Domain;

public class TaskDependency : EntityBase<Guid>
{
    public Guid BlockerTaskId { get; set; }
    public TaskItem? BlockerTask { get; set; }
    public Guid BlockedTaskId { get; set; }
    public TaskItem? BlockedTask { get; set; }
    public DependencyType DependencyType { get; set; } = DependencyType.FinishToStart;
}
