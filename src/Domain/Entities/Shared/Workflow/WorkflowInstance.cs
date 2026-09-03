namespace Domain;

public class WorkflowInstance : AuditableEntityBase<Guid>
{
    public Guid TemplateId { get; set; }
    public string EntityType { get; set; } = string.Empty;
    public Guid EntityId { get; set; }
    public int CurrentStep { get; set; } = 1;
    public WorkflowInstanceStatus Status { get; set; } = WorkflowInstanceStatus.InProgress;
    public DateTimeOffset? CompletedAt { get; set; }

    public WorkflowTemplate Template { get; set; } = null!;
    public ICollection<WorkflowTask> Tasks { get; set; } = [];
}
