namespace Application;

public sealed class SubmitRecruitmentRequestCommandHandler(
    IUnitOfWork unitOfWork,
    IApprovalWorkflowService workflowService)
    : IRequestHandler<SubmitRecruitmentRequestCommand, Unit>
{
    public async Task<Unit> Handle(SubmitRecruitmentRequestCommand cmd, CancellationToken ct)
    {
        var request = await unitOfWork.Repository<RecruitmentRequest>()
            .FindTrackedAsync(r => r.Id == cmd.RequestId, ct)
            ?? throw new NotFoundException(ExceptionMessages.NotFound("RecruitmentRequest", cmd.RequestId));

        if (request.Status != RecruitmentRequestStatus.Draft &&
            request.Status != RecruitmentRequestStatus.NeedMoreInfo)
            throw new BadRequestException("Chỉ có thể gửi phiếu ở trạng thái Draft hoặc NeedMoreInfo.");

        WorkflowScopeType scopeType;
        Guid? scopeEntityId;

        if (request.RequestContext == RecruitmentRequestContext.Store)
        {
            var store = await unitOfWork.Repository<Store>()
                .FindAsync(s => s.Id == request.StoreId!.Value, ct)
                ?? throw new NotFoundException(ExceptionMessages.NotFound("Store", request.StoreId!.Value));

            scopeType = WorkflowScopeType.Region;
            scopeEntityId = store.RegionId;
        }
        else
        {
            scopeType = WorkflowScopeType.Department;
            scopeEntityId = request.DepartmentId;
        }

        var instance = await workflowService.StartAsync("RecruitmentRequest", request.Id, scopeType, scopeEntityId, ct);

        request.Status = RecruitmentRequestStatus.PendingLevel1Approval;
        request.WorkflowInstanceId = instance.Id;

        await unitOfWork.EnsureSaveAsync(ct);
        return Unit.Value;
    }
}
