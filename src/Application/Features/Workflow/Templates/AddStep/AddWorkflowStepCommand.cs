namespace Application;

public sealed record AddWorkflowStepCommand(
    Guid TemplateId,
    int StepOrder,
    string StepName,
    WorkflowApproverType ApproverType,
    Guid? ApproverId
) : IRequest<Guid>;
