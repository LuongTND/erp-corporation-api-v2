namespace Application;

public sealed class RequestMoreInfoRecruitmentCommandHandler(
    IUnitOfWork unitOfWork,
    IUserContext userContext,
    IApprovalWorkflowService workflowService)
    : IRequestHandler<RequestMoreInfoRecruitmentCommand, Unit>
{
    public async Task<Unit> Handle(RequestMoreInfoRecruitmentCommand cmd, CancellationToken ct)
    {
        var request = await unitOfWork.Repository<RecruitmentRequest>()
            .FindTrackedAsync(r => r.Id == cmd.RequestId, ct)
            ?? throw new NotFoundException(ExceptionMessages.NotFound("RecruitmentRequest", cmd.RequestId));

        if (request.Status != RecruitmentRequestStatus.PendingApproval)
            throw new BadRequestException("Chỉ có thể yêu cầu thêm thông tin khi phiếu đang chờ duyệt.");

        if (!request.WorkflowInstanceId.HasValue)
            throw new BadRequestException("Phiếu chưa có workflow instance.");

        // Guard AssignedTo nằm trong CancelAsync — chỉ người đang giữ pending task mới cancel được
        await workflowService.CancelAsync(request.WorkflowInstanceId.Value, userContext.UserId, ct);

        request.Status = RecruitmentRequestStatus.NeedMoreInfo;
        request.NeedMoreInfoNote = cmd.NeedMoreInfoNote;
        request.WorkflowInstanceId = null;

        await unitOfWork.EnsureSaveAsync(ct);
        return Unit.Value;
    }
}
