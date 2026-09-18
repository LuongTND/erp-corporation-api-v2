namespace Application;

public sealed record DeleteWorkflowTemplateCommand(Guid TemplateId) : IRequest<Unit>;
