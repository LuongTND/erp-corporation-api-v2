namespace Application;

public interface IApprovalWorkflowService
{
    Task<WorkflowInstance> StartAsync(string entityType, Guid entityId, WorkflowScopeType scopeType, Guid? scopeEntityId, Guid creatorUserId, CancellationToken ct = default);
    Task ApproveAsync(Guid instanceId, Guid actorUserId, string? note, CancellationToken ct = default);
    Task RejectAsync(Guid instanceId, Guid actorUserId, string note, CancellationToken ct = default);
    Task CancelAsync(Guid instanceId, Guid actorUserId, CancellationToken ct = default);
}
