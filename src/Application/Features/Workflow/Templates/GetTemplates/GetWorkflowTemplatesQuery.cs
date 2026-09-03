namespace Application;

public sealed record GetWorkflowTemplatesQuery(string? EntityType = null) : IRequest<IReadOnlyList<WorkflowTemplateResponse>>;
