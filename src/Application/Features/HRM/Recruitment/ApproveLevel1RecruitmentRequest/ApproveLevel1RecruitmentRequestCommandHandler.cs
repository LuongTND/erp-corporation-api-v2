namespace Application;

public sealed class ApproveLevel1RecruitmentRequestCommandHandler(
    IUnitOfWork unitOfWork,
    IUserContext userContext,
    IApprovalWorkflowService workflowService)
    : IRequestHandler<ApproveLevel1RecruitmentRequestCommand, Unit>
{
    public async Task<Unit> Handle(ApproveLevel1RecruitmentRequestCommand cmd, CancellationToken ct)
    {
        var request = await unitOfWork.Repository<RecruitmentRequest>()
            .FindTrackedAsync(r => r.Id == cmd.RequestId, ct)
            ?? throw new NotFoundException(ExceptionMessages.NotFound("RecruitmentRequest", cmd.RequestId));

        if (request.Status != RecruitmentRequestStatus.PendingLevel1Approval)
            throw new BadRequestException("Chỉ có thể duyệt cấp 1 khi phiếu đang ở trạng thái chờ duyệt cấp 1.");

        if (!request.WorkflowInstanceId.HasValue)
            throw new BadRequestException("Phiếu chưa có workflow instance.");

        await workflowService.ApproveAsync(request.WorkflowInstanceId.Value, userContext.UserId, cmd.Note, ct);

        // double-write: FE hiển thị timeline không cần join WorkflowTask
        request.Status = RecruitmentRequestStatus.PendingLevel2Approval;
        request.Level1ApproverId = userContext.UserId;
        request.Level1ApprovedAt = DateTimeOffset.UtcNow;
        request.Level1Note = cmd.Note;

        await unitOfWork.EnsureSaveAsync(ct);
        return Unit.Value;
    }
}
