namespace Application;

public sealed class ReorderWorkflowStepsCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<ReorderWorkflowStepsCommand, Unit>
{
    public async Task<Unit> Handle(ReorderWorkflowStepsCommand cmd, CancellationToken ct)
    {
        // Kiểm tra template tồn tại
        _ = await unitOfWork.Repository<WorkflowTemplate>()
            .FindAsync(t => t.Id == cmd.TemplateId, ct)
            ?? throw new NotFoundException(ExceptionMessages.NotFound("WorkflowTemplate", cmd.TemplateId));

        // Load tất cả step của template này (tracked để EF theo dõi thay đổi)
        var steps = await unitOfWork.Repository<WorkflowTemplateStep>()
            .GetAllTrackedAsync(s => s.TemplateId == cmd.TemplateId, ct);

        // Validate: tất cả StepId trong request phải thuộc template này
        var stepDict = steps.ToDictionary(s => s.Id);
        foreach (var item in cmd.Items)
        {
            if (!stepDict.ContainsKey(item.StepId))
                throw new BadRequestException($"Step '{item.StepId}' không thuộc template này.");
        }

        // Áp dụng StepOrder mới — FE gửi lên toàn bộ danh sách sau khi drag-and-drop
        foreach (var item in cmd.Items)
            stepDict[item.StepId].StepOrder = item.StepOrder;

        await unitOfWork.EnsureSaveAsync(ct);
        return Unit.Value;
    }
}
