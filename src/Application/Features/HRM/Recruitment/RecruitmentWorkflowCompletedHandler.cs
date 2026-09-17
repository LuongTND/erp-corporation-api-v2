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

        request.Status = notification.FinalStatus switch
        {
            WorkflowInstanceStatus.Completed => RecruitmentRequestStatus.Approved,
            WorkflowInstanceStatus.Rejected  => RecruitmentRequestStatus.Rejected,
            WorkflowInstanceStatus.Cancelled when request.Status == RecruitmentRequestStatus.PendingApproval
                => RecruitmentRequestStatus.Cancelled,
            _ => request.Status, // NeedMoreInfo flow: status đã set trước khi cancel, giữ nguyên
        };

        await unitOfWork.EnsureSaveAsync(ct);
    }
}
