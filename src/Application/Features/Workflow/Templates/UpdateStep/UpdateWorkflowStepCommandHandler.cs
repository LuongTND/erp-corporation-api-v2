namespace Application;

public sealed class UpdateWorkflowStepCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateWorkflowStepCommand, Unit>
{
    public async Task<Unit> Handle(UpdateWorkflowStepCommand cmd, CancellationToken ct)
    {
        var step = await unitOfWork.Repository<WorkflowTemplateStep>()
            .FindTrackedAsync(s => s.Id == cmd.StepId && s.TemplateId == cmd.TemplateId, ct)
            ?? throw new NotFoundException(ExceptionMessages.NotFound("WorkflowTemplateStep", cmd.StepId));

        if (cmd.ApproverType == WorkflowApproverType.SpecificUser && !cmd.ApproverId.HasValue)
            throw new BadRequestException("SpecificUser yêu cầu ApproverId.");

        step.StepName = cmd.StepName;
        step.ApproverType = cmd.ApproverType;
        step.ApproverId = cmd.ApproverType == WorkflowApproverType.SpecificUser ? cmd.ApproverId : null;

        await unitOfWork.EnsureSaveAsync(ct);
        return Unit.Value;
    }
}
