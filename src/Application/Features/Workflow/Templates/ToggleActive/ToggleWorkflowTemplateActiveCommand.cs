namespace Application;

public sealed record ToggleWorkflowTemplateActiveCommand(Guid TemplateId) : IRequest<Unit>;
