namespace Application;

public sealed class AddWorkflowStepCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<AddWorkflowStepCommand, Guid>
{
    public async Task<Guid> Handle(AddWorkflowStepCommand cmd, CancellationToken ct)
    {
        var templateExists = await unitOfWork.Repository<WorkflowTemplate>()
            .AnyAsync(t => t.Id == cmd.TemplateId, ct);
        if (!templateExists)
            throw new NotFoundException(ExceptionMessages.NotFound("WorkflowTemplate", cmd.TemplateId));

        if (cmd.ApproverType == WorkflowApproverType.SpecificUser && !cmd.ApproverId.HasValue)
            throw new BadRequestException("SpecificUser yêu cầu ApproverId.");

        var orderConflict = await unitOfWork.Repository<WorkflowTemplateStep>()
            .AnyAsync(s => s.TemplateId == cmd.TemplateId && s.StepOrder == cmd.StepOrder, ct);
        if (orderConflict)
            throw new ConflictException($"Template đã có bước {cmd.StepOrder}.");

        var step = new WorkflowTemplateStep
        {
            Id = Guid.NewGuid(),
            TemplateId = cmd.TemplateId,
            StepOrder = cmd.StepOrder,
            StepName = cmd.StepName,
            ApproverType = cmd.ApproverType,
            ApproverId = cmd.ApproverType == WorkflowApproverType.SpecificUser ? cmd.ApproverId : null,
            CreatedAt = DateTimeOffset.UtcNow,
        };

        await unitOfWork.Repository<WorkflowTemplateStep>().AddAsync(step);
        await unitOfWork.EnsureSaveAsync(ct);
        return step.Id;
    }
}
