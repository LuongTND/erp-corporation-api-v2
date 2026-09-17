namespace Application;

public sealed class CancelWorkflowInstanceCommandHandler(IApprovalWorkflowService workflowService)
    : IRequestHandler<CancelWorkflowInstanceCommand, Unit>
{
    public async Task<Unit> Handle(CancelWorkflowInstanceCommand cmd, CancellationToken ct)
    {
        await workflowService.CancelAsync(cmd.InstanceId, cmd.ActorUserId, ct);
        return Unit.Value;
    }
}
