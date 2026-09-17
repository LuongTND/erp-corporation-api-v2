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

        if (request.Status != RecruitmentRequestStatus.PendingApproval)
            throw new BadRequestException("Phiếu không ở trạng thái chờ duyệt.");

        if (!request.WorkflowInstanceId.HasValue)
            throw new BadRequestException("Phiếu chưa có workflow instance.");

        // Engine tự guard AssignedTo + advance step; nếu complete → publish notification → RecruitmentWorkflowCompletedHandler set Approved
        // workflowService.ApproveAsync đã gọi EnsureSaveAsync bên trong, không gọi lại ở đây
        await workflowService.ApproveAsync(request.WorkflowInstanceId.Value, userContext.UserId, cmd.Note, ct);
        return Unit.Value;
    }
}
