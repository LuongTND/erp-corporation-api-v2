namespace Application;

public sealed class RejectRecruitmentRequestCommandHandler(
    IUnitOfWork unitOfWork,
    IUserContext userContext,
    IApprovalWorkflowService workflowService)
    : IRequestHandler<RejectRecruitmentRequestCommand, Unit>
{
    public async Task<Unit> Handle(RejectRecruitmentRequestCommand cmd, CancellationToken ct)
    {
        var request = await unitOfWork.Repository<RecruitmentRequest>()
            .FindTrackedAsync(r => r.Id == cmd.RequestId, ct)
            ?? throw new NotFoundException(ExceptionMessages.NotFound("RecruitmentRequest", cmd.RequestId));

        if (request.Status is not (RecruitmentRequestStatus.PendingLevel1Approval or RecruitmentRequestStatus.PendingLevel2Approval))
            throw new BadRequestException("Chỉ có thể từ chối phiếu đang chờ duyệt.");

        if (!request.WorkflowInstanceId.HasValue)
            throw new BadRequestException("Phiếu chưa có workflow instance.");

        await workflowService.RejectAsync(request.WorkflowInstanceId.Value, userContext.UserId, cmd.RejectionNote, ct);

        // RejectionNote lưu trên entity để filter/hiển thị không cần join
        request.RejectionNote = cmd.RejectionNote;

        await unitOfWork.EnsureSaveAsync(ct);
        return Unit.Value;
    }
}
