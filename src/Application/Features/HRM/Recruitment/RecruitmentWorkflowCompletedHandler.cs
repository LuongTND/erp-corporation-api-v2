namespace Application;

public sealed class RecruitmentWorkflowCompletedHandler(IUnitOfWork unitOfWork)
    : INotificationHandler<WorkflowCompletedNotification>
{
    public async Task Handle(WorkflowCompletedNotification notification, CancellationToken ct)
    {
        if (notification.EntityType != "RecruitmentRequest") return;

        var request = await unitOfWork.Repository<RecruitmentRequest>()
            .FindTrackedAsync(r => r.Id == notification.EntityId, ct)
            ?? throw new NotFoundException(ExceptionMessages.NotFound("RecruitmentRequest", notification.EntityId));

        request.Status = notification.FinalStatus == WorkflowInstanceStatus.Completed
            ? RecruitmentRequestStatus.Approved
            : RecruitmentRequestStatus.Rejected;

        await unitOfWork.EnsureSaveAsync(ct);
    }
}
