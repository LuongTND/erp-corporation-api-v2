namespace Application;

public sealed record GetWorkflowTemplateDetailQuery(Guid TemplateId) : IRequest<WorkflowTemplateResponse>;
