namespace Application;

public sealed class UpdateWorkflowTemplateCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateWorkflowTemplateCommand, Unit>
{
    public async Task<Unit> Handle(UpdateWorkflowTemplateCommand cmd, CancellationToken ct)
    {
        var template = await unitOfWork.Repository<WorkflowTemplate>()
            .FindTrackedAsync(t => t.Id == cmd.TemplateId, ct)
            ?? throw new NotFoundException(ExceptionMessages.NotFound("WorkflowTemplate", cmd.TemplateId));

        // --- Cập nhật tên (nếu được gửi lên) ---
        if (cmd.Name is not null)
            template.Name = cmd.Name;

        // --- Cập nhật scope (nếu ScopeType được gửi lên) ---
        // ScopeEntityId chỉ có ý nghĩa khi ScopeType thay đổi, nên cả hai đi cùng nhau
        if (cmd.ScopeType.HasValue)
        {
            // Conflict check: không được trùng EntityType + ScopeType + ScopeEntityId với template khác
            var conflict = await unitOfWork.Repository<WorkflowTemplate>()
                .AnyAsync(t =>
                    t.Id != cmd.TemplateId &&
                    t.EntityType == template.EntityType &&
                    t.ScopeType == cmd.ScopeType.Value &&
                    t.ScopeEntityId == cmd.ScopeEntityId, ct);

            if (conflict)
                throw new ConflictException($"Đã tồn tại template khác cho '{template.EntityType}' với scope này.");

            template.ScopeType = cmd.ScopeType.Value;
            template.ScopeEntityId = cmd.ScopeEntityId;
        }

        // --- Diff steps (nếu được gửi lên) ---
        if (cmd.Steps is not null)
        {
            var existingSteps = await unitOfWork.Repository<WorkflowTemplateStep>()
                .GetAllTrackedAsync(s => s.TemplateId == cmd.TemplateId, ct);

            var existingDict = existingSteps.ToDictionary(s => s.Id);
            var incomingIds = cmd.Steps.Where(s => s.Id.HasValue).Select(s => s.Id!.Value).ToHashSet();

            // Xóa bước không còn trong danh sách mới
            foreach (var step in existingSteps.Where(s => !incomingIds.Contains(s.Id)))
                await unitOfWork.Repository<WorkflowTemplateStep>().RemoveAsync(step);

            // Upsert: update nếu Id có giá trị, tạo mới nếu Id = null
            foreach (var item in cmd.Steps)
            {
                if (item.Id.HasValue && existingDict.TryGetValue(item.Id.Value, out var existing))
                {
                    existing.StepOrder = item.StepOrder;
                    existing.StepName = item.StepName;
                    existing.ApproverType = item.ApproverType;
                    existing.ApproverId = item.ApproverId;
                }
                else
                {
                    await unitOfWork.Repository<WorkflowTemplateStep>().AddAsync(new WorkflowTemplateStep
                    {
                        Id = Guid.NewGuid(),
                        TemplateId = cmd.TemplateId,
                        StepOrder = item.StepOrder,
                        StepName = item.StepName,
                        ApproverType = item.ApproverType,
                        ApproverId = item.ApproverId,
                    });
                }
            }
        }

        await unitOfWork.EnsureSaveAsync(ct);
        return Unit.Value;
    }
}
