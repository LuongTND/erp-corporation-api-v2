namespace Application;

public sealed record CancelWorkflowInstanceCommand(Guid InstanceId, Guid ActorUserId) : IRequest<Unit>;
