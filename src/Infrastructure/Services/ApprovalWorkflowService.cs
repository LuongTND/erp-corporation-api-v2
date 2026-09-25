namespace Infrastructure;

[RegisterService(typeof(IApprovalWorkflowService))]
public sealed class ApprovalWorkflowService(IUnitOfWork unitOfWork, IPublisher publisher) : IApprovalWorkflowService
{
    public async Task<WorkflowInstance> StartAsync(string entityType, Guid entityId, WorkflowScopeType scopeType, Guid? scopeEntityId, Guid creatorUserId, CancellationToken ct = default)
    {
        var template = await ResolveTemplateAsync(entityType, scopeType, scopeEntityId, ct);

        var steps = await unitOfWork.Repository<WorkflowTemplateStep>()
            .GetAllAsync(s => s.TemplateId == template.Id, ct);

        if (steps.Count == 0)
            throw new BadRequestException($"Template '{template.Name}' chưa có bước duyệt nào.");

        var orderedSteps = steps.OrderBy(s => s.StepOrder).ToList();
        var firstStep = orderedSteps.First();

        // Resolve trước khi tạo instance — nếu thiếu approver thì báo lỗi ngay, không tạo dữ liệu thừa
        var (assignedTo, assignedToRoleId) = await ResolveApproverAsync(firstStep, scopeType, scopeEntityId, ct);

        var instance = new WorkflowInstance
        {
            Id = Guid.NewGuid(),
            TemplateId = template.Id,
            EntityType = entityType,
            EntityId = entityId,
            ScopeType = scopeType,
            ScopeEntityId = scopeEntityId,
            CurrentStep = firstStep.StepOrder,
            Status = WorkflowInstanceStatus.InProgress,
            CreatedAt = DateTimeOffset.UtcNow,
            CreatedBy = creatorUserId,
        };
        await unitOfWork.Repository<WorkflowInstance>().AddAsync(instance);

        // Self-approval: người tạo == người duyệt L1 → auto-approve L1, chuyển thẳng lên L2
        // Chỉ áp dụng khi task assign cho user cụ thể (không áp dụng cho role task)
        if (assignedTo.HasValue && assignedTo == creatorUserId)
        {
            var autoTask = new WorkflowTask
            {
                Id = Guid.NewGuid(),
                InstanceId = instance.Id,
                StepOrder = firstStep.StepOrder,
                StepName = firstStep.StepName,
                AssignedTo = assignedTo,
                AssignedToRoleId = null,
                Status = WorkflowTaskStatus.Approved,
                Note = "Tự động duyệt — người tạo là người duyệt cấp này.",
                ActedAt = DateTimeOffset.UtcNow,
                CreatedAt = DateTimeOffset.UtcNow,
            };
            await unitOfWork.Repository<WorkflowTask>().AddAsync(autoTask);

            var nextStep = orderedSteps.FirstOrDefault(s => s.StepOrder > firstStep.StepOrder);
            if (nextStep is null)
            {
                instance.Status = WorkflowInstanceStatus.Completed;
                instance.CompletedAt = DateTimeOffset.UtcNow;
                await unitOfWork.EnsureSaveAsync(ct);
                await publisher.Publish(new WorkflowCompletedNotification(entityType, entityId, WorkflowInstanceStatus.Completed), ct);
                return instance;
            }

            var (nextUserId, nextRoleId) = await ResolveApproverAsync(nextStep, scopeType, scopeEntityId, ct);
            instance.CurrentStep = nextStep.StepOrder;

            var nextTask = new WorkflowTask
            {
                Id = Guid.NewGuid(),
                InstanceId = instance.Id,
                StepOrder = nextStep.StepOrder,
                StepName = nextStep.StepName,
                AssignedTo = nextUserId,
                AssignedToRoleId = nextRoleId,
                Status = WorkflowTaskStatus.Pending,
                CreatedAt = DateTimeOffset.UtcNow,
            };
            await unitOfWork.Repository<WorkflowTask>().AddAsync(nextTask);
            await unitOfWork.EnsureSaveAsync(ct);
            return instance;
        }

        var task = new WorkflowTask
        {
            Id = Guid.NewGuid(),
            InstanceId = instance.Id,
            StepOrder = firstStep.StepOrder,
            StepName = firstStep.StepName,
            AssignedTo = assignedTo,
            AssignedToRoleId = assignedToRoleId,
            Status = WorkflowTaskStatus.Pending,
            CreatedAt = DateTimeOffset.UtcNow,
        };
        await unitOfWork.Repository<WorkflowTask>().AddAsync(task);

        await unitOfWork.EnsureSaveAsync(ct);
        return instance;
    }

    public async Task ApproveAsync(Guid instanceId, Guid actorUserId, string? note, CancellationToken ct = default)
    {
        var instance = await unitOfWork.Repository<WorkflowInstance>()
            .FindTrackedAsync(i => i.Id == instanceId, ct)
            ?? throw new NotFoundException(ExceptionMessages.NotFound("WorkflowInstance", instanceId));

        var pendingTask = await unitOfWork.Repository<WorkflowTask>()
            .FindTrackedAsync(t => t.InstanceId == instanceId && t.StepOrder == instance.CurrentStep && t.Status == WorkflowTaskStatus.Pending, ct)
            ?? throw new NotFoundException("Không tìm thấy task đang chờ duyệt.");

        var actorHasRole = pendingTask.AssignedToRoleId.HasValue && await unitOfWork.Repository<UserRole>()
            .AnyAsync(ur => ur.RoleId == pendingTask.AssignedToRoleId.Value && ur.UserId == actorUserId
                            && ur.IsActive && ur.RevokedAt == null
                            && (ur.ExpiresAt == null || ur.ExpiresAt > DateTimeOffset.UtcNow), ct);

        if (pendingTask.AssignedTo != actorUserId && !actorHasRole)
            throw new ForbiddenException("Bạn không có quyền duyệt task này.");

        pendingTask.Status = WorkflowTaskStatus.Approved;
        pendingTask.Note = note;
        pendingTask.ActedAt = DateTimeOffset.UtcNow;
        pendingTask.ActedByUserId = actorUserId;

        var steps = await unitOfWork.Repository<WorkflowTemplateStep>()
            .GetAllAsync(s => s.TemplateId == instance.TemplateId, ct);

        var nextStep = steps.OrderBy(s => s.StepOrder)
            .FirstOrDefault(s => s.StepOrder > instance.CurrentStep);

        if (nextStep is null)
        {
            instance.Status = WorkflowInstanceStatus.Completed;
            instance.CompletedAt = DateTimeOffset.UtcNow;
            await unitOfWork.EnsureSaveAsync(ct);
            await publisher.Publish(new WorkflowCompletedNotification(instance.EntityType, instance.EntityId, WorkflowInstanceStatus.Completed), ct);
            return;
        }

        var (nextUserId, nextRoleId) = await ResolveApproverAsync(nextStep, instance.ScopeType, instance.ScopeEntityId, ct);

        instance.CurrentStep = nextStep.StepOrder;

        var nextTask = new WorkflowTask
        {
            Id = Guid.NewGuid(),
            InstanceId = instance.Id,
            StepOrder = nextStep.StepOrder,
            StepName = nextStep.StepName,
            AssignedTo = nextUserId,
            AssignedToRoleId = nextRoleId,
            Status = WorkflowTaskStatus.Pending,
            CreatedAt = DateTimeOffset.UtcNow,
        };
        await unitOfWork.Repository<WorkflowTask>().AddAsync(nextTask);
        await unitOfWork.EnsureSaveAsync(ct);
    }

    public async Task RejectAsync(Guid instanceId, Guid actorUserId, string note, CancellationToken ct = default)
    {
        var instance = await unitOfWork.Repository<WorkflowInstance>()
            .FindTrackedAsync(i => i.Id == instanceId, ct)
            ?? throw new NotFoundException(ExceptionMessages.NotFound("WorkflowInstance", instanceId));

        var pendingTask = await unitOfWork.Repository<WorkflowTask>()
            .FindTrackedAsync(t => t.InstanceId == instanceId && t.StepOrder == instance.CurrentStep && t.Status == WorkflowTaskStatus.Pending, ct)
            ?? throw new NotFoundException("Không tìm thấy task đang chờ duyệt.");

        var actorHasRoleForReject = pendingTask.AssignedToRoleId.HasValue && await unitOfWork.Repository<UserRole>()
            .AnyAsync(ur => ur.RoleId == pendingTask.AssignedToRoleId.Value && ur.UserId == actorUserId
                            && ur.IsActive && ur.RevokedAt == null
                            && (ur.ExpiresAt == null || ur.ExpiresAt > DateTimeOffset.UtcNow), ct);

        if (pendingTask.AssignedTo != actorUserId && !actorHasRoleForReject)
            throw new ForbiddenException("Bạn không có quyền từ chối task này.");

        pendingTask.Status = WorkflowTaskStatus.Rejected;
        pendingTask.Note = note;
        pendingTask.ActedAt = DateTimeOffset.UtcNow;
        pendingTask.ActedByUserId = actorUserId;

        instance.Status = WorkflowInstanceStatus.Rejected;
        instance.CompletedAt = DateTimeOffset.UtcNow;

        await unitOfWork.EnsureSaveAsync(ct);
        await publisher.Publish(new WorkflowCompletedNotification(instance.EntityType, instance.EntityId, WorkflowInstanceStatus.Rejected), ct);
    }

    public async Task CancelAsync(Guid instanceId, Guid actorUserId, CancellationToken ct = default)
    {
        var instance = await unitOfWork.Repository<WorkflowInstance>()
            .FindTrackedAsync(i => i.Id == instanceId, ct)
            ?? throw new NotFoundException(ExceptionMessages.NotFound("WorkflowInstance", instanceId));

        if (instance.Status != WorkflowInstanceStatus.InProgress)
            throw new BadRequestException("Chỉ có thể hủy instance đang InProgress.");

        // Guard: chỉ người đang giữ pending task (approver) hoặc người tạo instance mới được cancel
        // Business rule "ai được cancel" do caller (handler) kiểm soát trước khi gọi vào đây
        var pendingTask = await unitOfWork.Repository<WorkflowTask>()
            .FindTrackedAsync(t => t.InstanceId == instanceId && t.Status == WorkflowTaskStatus.Pending, ct);

        var actorHasRoleForCancel = pendingTask is { AssignedToRoleId: not null } && await unitOfWork.Repository<UserRole>()
            .AnyAsync(ur => ur.RoleId == pendingTask.AssignedToRoleId.Value && ur.UserId == actorUserId
                            && ur.IsActive && ur.RevokedAt == null
                            && (ur.ExpiresAt == null || ur.ExpiresAt > DateTimeOffset.UtcNow), ct);

        if (pendingTask is not null && pendingTask.AssignedTo != actorUserId && !actorHasRoleForCancel && instance.CreatedBy != actorUserId)
            throw new ForbiddenException("Bạn không có quyền hủy instance này.");

        if (pendingTask is not null)
        {
            pendingTask.Status = WorkflowTaskStatus.Rejected;
            pendingTask.Note = "Đã hủy.";
            pendingTask.ActedAt = DateTimeOffset.UtcNow;
        }

        instance.Status = WorkflowInstanceStatus.Cancelled;
        instance.CompletedAt = DateTimeOffset.UtcNow;
        await unitOfWork.EnsureSaveAsync(ct);
        await publisher.Publish(new WorkflowCompletedNotification(instance.EntityType, instance.EntityId, WorkflowInstanceStatus.Cancelled), ct);
    }

    /// <summary>
    /// Tìm template phù hợp theo 3 mức ưu tiên:
    ///   1. Khớp chính xác  → ví dụ: template riêng cho vùng Hòa Khánh
    ///   2. Wildcard cùng scope → template "mọi vùng" (ScopeEntityId = null)
    ///   3. Toàn công ty (All) → fallback cuối nếu không có template nào khớp
    /// </summary>
    private async Task<WorkflowTemplate> ResolveTemplateAsync(string entityType, WorkflowScopeType scopeType, Guid? scopeEntityId, CancellationToken ct)
    {
        var repo = unitOfWork.Repository<WorkflowTemplate>();

        // Mức 1: khớp chính xác (ví dụ: Region + Hòa Khánh)
        var template = await repo.FindAsync(
            t => t.EntityType == entityType && t.ScopeType == scopeType && t.ScopeEntityId == scopeEntityId && t.IsActive, ct);

        // Mức 2: wildcard cùng loại scope — template tạo với "Tất cả vùng/phòng ban" (ScopeEntityId = null)
        if (template is null && scopeType != WorkflowScopeType.All && scopeEntityId.HasValue)
            template = await repo.FindAsync(
                t => t.EntityType == entityType && t.ScopeType == scopeType && t.ScopeEntityId == null && t.IsActive, ct);

        // Mức 3: fallback toàn công ty
        if (template is null && scopeType != WorkflowScopeType.All)
            template = await repo.FindAsync(
                t => t.EntityType == entityType && t.ScopeType == WorkflowScopeType.All && t.ScopeEntityId == null && t.IsActive, ct);

        return template ?? throw new BadRequestException($"Chưa cấu hình workflow cho '{entityType}'. Vui lòng liên hệ quản trị viên.");
    }

    /// <summary>
    /// Xác định người duyệt cho một step. Trả về (userId, roleId) — đúng một cái có giá trị:
    ///   - SpecificUser    → (userId, null)
    ///   - Role            → (null, roleId) — bất kỳ ai mang role đó đều có thể duyệt
    ///   - OrgUnitManager  → (managerId, null) — manager cụ thể của vùng/phòng ban
    /// scopeEntityId là của phiếu thực tế (regionId/deptId), không phải template.
    /// </summary>
    private async Task<(Guid? UserId, Guid? RoleId)> ResolveApproverAsync(WorkflowTemplateStep step, WorkflowScopeType scopeType, Guid? scopeEntityId, CancellationToken ct)
    {
        if (step.ApproverType == WorkflowApproverType.SpecificUser)
            return (step.ApproverId ?? throw new BadRequestException($"Bước '{step.StepName}' thiếu người duyệt."), null);

        if (step.ApproverType == WorkflowApproverType.Role)
        {
            var roleId = step.ApproverId ?? throw new BadRequestException($"Bước '{step.StepName}' thiếu vai trò.");
            // Validate role có member active không — báo lỗi sớm thay vì tạo task treo
            var hasMember = await unitOfWork.Repository<UserRole>()
                .AnyAsync(ur => ur.RoleId == roleId && ur.IsActive && ur.RevokedAt == null
                                && (ur.ExpiresAt == null || ur.ExpiresAt > DateTimeOffset.UtcNow), ct);
            if (!hasMember)
                throw new BadRequestException($"Vai trò ở step '{step.StepName}' chưa có thành viên active nào.");
            return (null, roleId);
        }

        // OrgUnitManager: lấy manager của vùng/phòng ban mà phiếu thuộc về
        var managerId = scopeType switch
        {
            WorkflowScopeType.Region when scopeEntityId.HasValue => await ResolveRegionApproverAsync(scopeEntityId.Value, ct),
            WorkflowScopeType.Department when scopeEntityId.HasValue => await ResolveDepartmentApproverAsync(scopeEntityId.Value, ct),
            _ => throw new BadRequestException($"Không thể resolve approver cho step '{step.StepName}'."),
        };
        return (managerId, null);
    }

    // Trả về ManagerId của vùng — bắt buộc phải gán trước khi tạo phiếu
    private async Task<Guid> ResolveRegionApproverAsync(Guid regionId, CancellationToken ct)
    {
        var region = await unitOfWork.Repository<Region>().FindAsync(r => r.Id == regionId, ct)
            ?? throw new NotFoundException(ExceptionMessages.NotFound("Region", regionId));
        return region.ManagerId ?? throw new BadRequestException($"Quản lý vùng {region.Name} chưa được gán.");
    }

    // Đi ngược cây phòng ban (tối đa 5 cấp) để tìm ManagerId gần nhất
    private async Task<Guid> ResolveDepartmentApproverAsync(Guid deptId, CancellationToken ct)
    {
        for (var depth = 0; depth < 5; depth++)
        {
            var dept = await unitOfWork.Repository<Department>().FindAsync(d => d.Id == deptId, ct)
                ?? throw new NotFoundException(ExceptionMessages.NotFound("Department", deptId));
            if (dept.ManagerId.HasValue) return dept.ManagerId.Value;
            if (!dept.ParentDepartmentId.HasValue) break;
            deptId = dept.ParentDepartmentId.Value; // leo lên phòng ban cha
        }
        throw new BadRequestException("Phòng ban chưa có trưởng phòng.");
    }
}
