namespace Application;

public sealed record CreateWorkflowTemplateCommand(
    string Name,
    string EntityType,
    WorkflowScopeType ScopeType,
    Guid? ScopeEntityId
) : IRequest<Guid>;
