namespace Application;

public sealed record DeleteWorkflowStepCommand(Guid TemplateId, Guid StepId) : IRequest<Unit>;
