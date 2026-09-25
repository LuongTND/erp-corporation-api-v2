namespace Application;

public sealed record WorkflowCompletedNotification(
    string EntityType,
    Guid EntityId,
    WorkflowInstanceStatus FinalStatus
) : INotification;
