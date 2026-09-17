namespace Application;

public sealed record GetWorkflowEntityTypesQuery : IRequest<IReadOnlyList<WorkflowEntityTypeItem>>;
