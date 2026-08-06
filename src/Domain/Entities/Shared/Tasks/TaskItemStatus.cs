namespace Domain;

public class TaskItemStatus : EntityBase<Guid>
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public int Order { get; set; }
    public bool IsSystem { get; set; }
    public bool IsFinalState { get; set; }
    public bool IsInitialState { get; set; }
    public bool IsActive { get; set; } = true;

    public ICollection<TaskItem> Tasks { get; set; } = [];
}
