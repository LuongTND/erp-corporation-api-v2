namespace Application;

public sealed record GetMyPendingTasksQuery(string? EntityType = null) : IRequest<IReadOnlyList<WorkflowTaskResponse>>;
