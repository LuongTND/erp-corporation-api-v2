namespace Infrastructure;

[RegisterService(typeof(IApprovalWorkflowService))]
public sealed class ApprovalWorkflowService(IUnitOfWork unitOfWork, IPublisher publisher) : IApprovalWorkflowService
{
    public async Task<WorkflowInstance> StartAsync(string entityType, Guid entityId, WorkflowScopeType scopeType, Guid? scopeEntityId, CancellationToken ct = default)
    {
        var template = await ResolveTemplateAsync(entityType, scopeType, scopeEntityId, ct);

        var steps = await unitOfWork.Repository<WorkflowTemplateStep>()
            .GetAllAsync(s => s.TemplateId == template.Id, ct);

        if (steps.Count == 0)
            throw new InvalidOperationException($"Template '{template.Name}' chưa có bước duyệt nào.");

        var firstStep = steps.OrderBy(s => s.StepOrder).First();
        var assignedTo = await ResolveApproverAsync(firstStep, scopeType, scopeEntityId, ct);

        var instance = new WorkflowInstance
        {
            Id = Guid.NewGuid(),
            TemplateId = template.Id,
            EntityType = entityType,
            EntityId = entityId,
            CurrentStep = firstStep.StepOrder,
            Status = WorkflowInstanceStatus.InProgress,
            CreatedAt = DateTimeOffset.UtcNow,
        };
        await unitOfWork.Repository<WorkflowInstance>().AddAsync(instance);

        var task = new WorkflowTask
        {
            Id = Guid.NewGuid(),
            InstanceId = instance.Id,
            StepOrder = firstStep.StepOrder,
            StepName = firstStep.StepName,
            AssignedTo = assignedTo,
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

        if (pendingTask.AssignedTo != actorUserId)
            throw new ForbiddenException("Bạn không có quyền duyệt task này.");

        pendingTask.Status = WorkflowTaskStatus.Approved;
        pendingTask.Note = note;
        pendingTask.ActedAt = DateTimeOffset.UtcNow;

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

        var template = await unitOfWork.Repository<WorkflowTemplate>()
            .FindAsync(t => t.Id == instance.TemplateId, ct)
            ?? throw new NotFoundException(ExceptionMessages.NotFound("WorkflowTemplate", instance.TemplateId));

        var assignedTo = await ResolveApproverAsync(nextStep, template.ScopeType, template.ScopeEntityId, ct);

        instance.CurrentStep = nextStep.StepOrder;

        var nextTask = new WorkflowTask
        {
            Id = Guid.NewGuid(),
            InstanceId = instance.Id,
            StepOrder = nextStep.StepOrder,
            StepName = nextStep.StepName,
            AssignedTo = assignedTo,
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

        if (pendingTask.AssignedTo != actorUserId)
            throw new ForbiddenException("Bạn không có quyền từ chối task này.");

        pendingTask.Status = WorkflowTaskStatus.Rejected;
        pendingTask.Note = note;
        pendingTask.ActedAt = DateTimeOffset.UtcNow;

        instance.Status = WorkflowInstanceStatus.Rejected;
        instance.CompletedAt = DateTimeOffset.UtcNow;

        await unitOfWork.EnsureSaveAsync(ct);
        await publisher.Publish(new WorkflowCompletedNotification(instance.EntityType, instance.EntityId, WorkflowInstanceStatus.Rejected), ct);
    }

    public async Task CancelAsync(Guid instanceId, CancellationToken ct = default)
    {
        var instance = await unitOfWork.Repository<WorkflowInstance>()
            .FindTrackedAsync(i => i.Id == instanceId, ct)
            ?? throw new NotFoundException(ExceptionMessages.NotFound("WorkflowInstance", instanceId));

        instance.Status = WorkflowInstanceStatus.Cancelled;
        instance.CompletedAt = DateTimeOffset.UtcNow;
        await unitOfWork.EnsureSaveAsync(ct);
    }

    private async Task<WorkflowTemplate> ResolveTemplateAsync(string entityType, WorkflowScopeType scopeType, Guid? scopeEntityId, CancellationToken ct)
    {
        var template = await unitOfWork.Repository<WorkflowTemplate>()
            .FindAsync(t => t.EntityType == entityType && t.ScopeType == scopeType && t.ScopeEntityId == scopeEntityId && t.IsActive, ct);

        if (template is null && scopeType != WorkflowScopeType.All)
            template = await unitOfWork.Repository<WorkflowTemplate>()
                .FindAsync(t => t.EntityType == entityType && t.ScopeType == WorkflowScopeType.All && t.ScopeEntityId == null && t.IsActive, ct);

        return template ?? throw new InvalidOperationException($"Chưa cấu hình workflow cho '{entityType}'. Vui lòng liên hệ quản trị viên.");
    }

    private async Task<Guid> ResolveApproverAsync(WorkflowTemplateStep step, WorkflowScopeType scopeType, Guid? scopeEntityId, CancellationToken ct)
    {
        if (step.ApproverType == WorkflowApproverType.SpecificUser)
            return step.ApproverId ?? throw new InvalidOperationException($"Step '{step.StepName}' thiếu ApproverId.");

        if (scopeType == WorkflowScopeType.Region && scopeEntityId.HasValue)
        {
            var region = await unitOfWork.Repository<Region>()
                .FindAsync(r => r.Id == scopeEntityId.Value, ct)
                ?? throw new NotFoundException(ExceptionMessages.NotFound("Region", scopeEntityId.Value));

            return region.ManagerId ?? throw new InvalidOperationException("Vùng chưa có giám sát viên.");
        }

        if (scopeType == WorkflowScopeType.Department && scopeEntityId.HasValue)
        {
            var deptId = scopeEntityId.Value;
            for (var depth = 0; depth < 5; depth++)
            {
                var dept = await unitOfWork.Repository<Department>()
                    .FindAsync(d => d.Id == deptId, ct)
                    ?? throw new NotFoundException(ExceptionMessages.NotFound("Department", deptId));

                if (dept.ManagerId.HasValue) return dept.ManagerId.Value;
                if (!dept.ParentDepartmentId.HasValue) break;
                deptId = dept.ParentDepartmentId.Value;
            }
            throw new InvalidOperationException("Phòng ban chưa có trưởng phòng.");
        }

        throw new InvalidOperationException($"Không thể resolve approver cho step '{step.StepName}'.");
    }
}
