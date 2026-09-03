namespace Application;

public sealed record GetInstanceTasksQuery(Guid InstanceId) : IRequest<IReadOnlyList<WorkflowTaskResponse>>;
