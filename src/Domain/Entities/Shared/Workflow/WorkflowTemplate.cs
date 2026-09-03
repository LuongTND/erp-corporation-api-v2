namespace Domain;

public class WorkflowTemplate : AuditableEntityBase<Guid>
{
    public string Name { get; set; } = string.Empty;
    public string EntityType { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public WorkflowScopeType ScopeType { get; set; }
    public Guid? ScopeEntityId { get; set; }

    public ICollection<WorkflowTemplateStep> Steps { get; set; } = [];
}
