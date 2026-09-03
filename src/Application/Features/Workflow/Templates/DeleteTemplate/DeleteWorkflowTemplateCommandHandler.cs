namespace Application;

public sealed class DeleteWorkflowTemplateCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<DeleteWorkflowTemplateCommand, Unit>
{
    public async Task<Unit> Handle(DeleteWorkflowTemplateCommand cmd, CancellationToken ct)
    {
        var template = await unitOfWork.Repository<WorkflowTemplate>()
            .FindAsync(t => t.Id == cmd.TemplateId, ct)
            ?? throw new NotFoundException(ExceptionMessages.NotFound("WorkflowTemplate", cmd.TemplateId));

        var hasActiveInstances = await unitOfWork.Repository<WorkflowInstance>()
            .AnyAsync(i => i.TemplateId == cmd.TemplateId && i.Status == WorkflowInstanceStatus.InProgress, ct);

        if (hasActiveInstances)
            throw new BadRequestException("Không thể xóa template đang có workflow instance đang chạy.");

        await unitOfWork.Repository<WorkflowTemplate>().RemoveAsync(template);
        await unitOfWork.EnsureSaveAsync(ct);
        return Unit.Value;
    }
}
