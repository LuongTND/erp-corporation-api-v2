namespace Application;

public sealed class CancelRecruitmentRequestCommandHandler(
    IUnitOfWork unitOfWork,
    IUserContext userContext,
    IApprovalWorkflowService workflowService)
    : IRequestHandler<CancelRecruitmentRequestCommand, Unit>
{
    public async Task<Unit> Handle(CancelRecruitmentRequestCommand cmd, CancellationToken ct)
    {
        var request = await unitOfWork.Repository<RecruitmentRequest>()
            .FindTrackedAsync(r => r.Id == cmd.RequestId, ct)
            ?? throw new NotFoundException(ExceptionMessages.NotFound("RecruitmentRequest", cmd.RequestId));

        if (request.RequestedByUserId != userContext.UserId)
            throw new ForbiddenException("Chỉ người tạo phiếu mới có thể huỷ.");

        if (request.Status is not (RecruitmentRequestStatus.Draft or RecruitmentRequestStatus.PendingApproval))
            throw new BadRequestException("Chỉ có thể huỷ phiếu ở trạng thái Draft hoặc đang chờ duyệt.");

        request.CancelNote = cmd.Note;

        if (request.Status == RecruitmentRequestStatus.Draft)
        {
            request.Status = RecruitmentRequestStatus.Cancelled;
        }
        else
        {
            if (!request.WorkflowInstanceId.HasValue)
                throw new BadRequestException("Phiếu chưa có workflow instance.");

            // CancelAsync publish notification → RecruitmentWorkflowCompletedHandler set request.Status = Cancelled
            await workflowService.CancelAsync(request.WorkflowInstanceId.Value, userContext.UserId, ct);
        }

        await unitOfWork.EnsureSaveAsync(ct);
        return Unit.Value;
    }
}
