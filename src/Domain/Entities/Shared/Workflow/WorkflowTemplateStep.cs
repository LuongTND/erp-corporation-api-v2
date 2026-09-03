namespace Domain;

public class WorkflowTemplateStep : EntityBase<Guid>
{
    public Guid TemplateId { get; set; }
    public int StepOrder { get; set; }
    public string StepName { get; set; } = string.Empty;
    public WorkflowApproverType ApproverType { get; set; }
    public Guid? ApproverId { get; set; }

    public WorkflowTemplate Template { get; set; } = null!;
}
