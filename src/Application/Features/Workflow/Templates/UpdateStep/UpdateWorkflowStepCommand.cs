namespace Application;

public sealed record UpdateWorkflowStepCommand(
    Guid TemplateId,
    Guid StepId,
    string StepName,
    WorkflowApproverType ApproverType,
    Guid? ApproverId
) : IRequest<Unit>;
