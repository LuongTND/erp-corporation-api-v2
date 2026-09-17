namespace Application;

public sealed class SubmitRecruitmentRequestCommandHandler(
    IUnitOfWork unitOfWork,
    IUserContext userContext,
    IApprovalWorkflowService workflowService)
    : IRequestHandler<SubmitRecruitmentRequestCommand, Unit>
{
    public async Task<Unit> Handle(SubmitRecruitmentRequestCommand cmd, CancellationToken ct)
    {
        var request = await unitOfWork.Repository<RecruitmentRequest>()
            .FindTrackedAsync(r => r.Id == cmd.RequestId, ct)
            ?? throw new NotFoundException(ExceptionMessages.NotFound("RecruitmentRequest", cmd.RequestId));

        // Chỉ cho gửi khi đang là Draft hoặc NeedMoreInfo (người tạo bổ sung thông tin xong gửi lại)
        if (request.Status != RecruitmentRequestStatus.Draft &&
            request.Status != RecruitmentRequestStatus.NeedMoreInfo)
            throw new BadRequestException("Chỉ có thể gửi phiếu ở trạng thái Draft hoặc NeedMoreInfo.");

        // Xác định scope để engine tìm đúng WorkflowTemplate:
        //
        //   Phiếu Store  → scopeType = Region, scopeEntityId = RegionId của cửa hàng đó
        //                   Engine tìm template: EntityType="RecruitmentRequest" + ScopeType=Region + ScopeEntityId=<regionId>
        //                   Không có template riêng cho vùng đó → fallback về ScopeType=All (template toàn công ty)
        //                   Approver Step 1 = Region.ManagerId (Giám sát vùng / Quản lý vùng)
        //
        //   Phiếu Office → scopeType = Department, scopeEntityId = DepartmentId của phiếu
        //                   Approver Step 1 = ManagerId của phòng ban, nếu null thì leo lên phòng cha (tối đa 5 cấp)
        //
        //   Muốn cấu hình approver khác nhau theo từng vùng: tạo template riêng ScopeType=Region cho từng RegionId.
        //   Muốn dùng chung một quy trình toàn quốc: giữ một template ScopeType=All là đủ (đang là cấu hình hiện tại).
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

        // workflowService.StartAsync thực hiện:
        //   1. Tìm WorkflowTemplate có EntityType="RecruitmentRequest" + scope khớp (ưu tiên scope cụ thể,
        //      fallback về ScopeType.All nếu không tìm thấy template riêng cho scope đó).
        //   2. Load tất cả WorkflowTemplateStep của template, sắp xếp theo StepOrder.
        //   3. Resolve approver cho Step đầu tiên:
        //        - ApproverType.SpecificUser → dùng ApproverId cố định trên step
        //        - ApproverType.RoleInScope  → tìm ManagerId của Region / Department (leo cây tối đa 5 cấp)
        //   4. Tạo WorkflowInstance (Status=InProgress, CurrentStep=step đầu).
        //   5. Self-approval check: nếu người gửi == approver Step 1 → task Step 1 tự động Approved,
        //      chuyển thẳng sang tạo task Step 2 (hoặc Completed nếu chỉ có 1 step).
        //   6. Tạo WorkflowTask đầu tiên (Pending) gán cho approver tìm được.
        //   Trả về WorkflowInstance vừa tạo.
        var instance = await workflowService.StartAsync("RecruitmentRequest", request.Id, scopeType, scopeEntityId, userContext.UserId, ct);

        // Sau khi workflow được khởi tạo, cập nhật trạng thái phiếu và lưu ID instance để tra cứu sau
        request.Status = RecruitmentRequestStatus.PendingApproval;
        request.WorkflowInstanceId = instance.Id;

        await unitOfWork.EnsureSaveAsync(ct);
        return Unit.Value;
    }
}
