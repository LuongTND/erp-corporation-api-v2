namespace Application;

public sealed class ApproveRecruitmentRequestCommandHandler(
    IUnitOfWork unitOfWork,
    IUserContext userContext,
    IApprovalWorkflowService workflowService)
    : IRequestHandler<ApproveRecruitmentRequestCommand, Unit>
{
    public async Task<Unit> Handle(ApproveRecruitmentRequestCommand cmd, CancellationToken ct)
    {
        var request = await unitOfWork.Repository<RecruitmentRequest>()
            .FindTrackedAsync(r => r.Id == cmd.RequestId, ct)
            ?? throw new NotFoundException(ExceptionMessages.NotFound("RecruitmentRequest", cmd.RequestId));

        if (request.Status != RecruitmentRequestStatus.PendingLevel2Approval)
            throw new BadRequestException("Chỉ có thể duyệt cấp 2 khi phiếu đang ở trạng thái chờ duyệt cấp 2.");

        if (!request.WorkflowInstanceId.HasValue)
            throw new BadRequestException("Phiếu chưa có workflow instance.");

        await workflowService.ApproveAsync(request.WorkflowInstanceId.Value, userContext.UserId, cmd.Note, ct);

        // double-write: Status Approved được set bởi RecruitmentWorkflowCompletedHandler,
        // Level2 fields set ở đây để đồng bộ trong cùng request
        request.Level2ApproverId = userContext.UserId;
        request.Level2ApprovedAt = DateTimeOffset.UtcNow;
        request.Level2Note = cmd.Note;

        await unitOfWork.EnsureSaveAsync(ct);
        return Unit.Value;
    }
}
