namespace Application;

public sealed class ToggleWorkflowTemplateActiveCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<ToggleWorkflowTemplateActiveCommand, Unit>
{
    public async Task<Unit> Handle(ToggleWorkflowTemplateActiveCommand cmd, CancellationToken ct)
    {
        var template = await unitOfWork.Repository<WorkflowTemplate>()
            .FindTrackedAsync(t => t.Id == cmd.TemplateId, ct)
            ?? throw new NotFoundException(ExceptionMessages.NotFound("WorkflowTemplate", cmd.TemplateId));

        // Đảo trạng thái: đang Active → Inactive, đang Inactive → Active
        // Khi IsActive=false, engine sẽ không tìm thấy template này nữa (ResolveTemplateAsync chỉ lấy IsActive=true)
        // → Submit phiếu sẽ fallback về template ScopeType=All, hoặc báo lỗi nếu không có fallback
        template.IsActive = !template.IsActive;

        await unitOfWork.EnsureSaveAsync(ct);
        return Unit.Value;
    }
}
