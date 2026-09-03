namespace Domain;

public class WorkflowTask : AuditableEntityBase<Guid>
{
    public Guid InstanceId { get; set; }
    public int StepOrder { get; set; }
    public string StepName { get; set; } = string.Empty;
    public Guid AssignedTo { get; set; }
    public WorkflowTaskStatus Status { get; set; } = WorkflowTaskStatus.Pending;
    public string? Note { get; set; }
    public DateTimeOffset? ActedAt { get; set; }

    public WorkflowInstance Instance { get; set; } = null!;
}
