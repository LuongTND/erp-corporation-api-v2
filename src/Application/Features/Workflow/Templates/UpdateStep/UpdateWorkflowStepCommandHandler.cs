namespace Application;

public sealed class UpdateWorkflowStepCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateWorkflowStepCommand, Unit>
{
    public async Task<Unit> Handle(UpdateWorkflowStepCommand cmd, CancellationToken ct)
    {
        var step = await unitOfWork.Repository<WorkflowTemplateStep>()
            .FindTrackedAsync(s => s.Id == cmd.StepId && s.TemplateId == cmd.TemplateId, ct)
            ?? throw new NotFoundException(ExceptionMessages.NotFound("WorkflowTemplateStep", cmd.StepId));

        if (cmd.ApproverType is WorkflowApproverType.SpecificUser or WorkflowApproverType.Role && !cmd.ApproverId.HasValue)
            throw new BadRequestException($"{cmd.ApproverType} yêu cầu ApproverId.");

        step.StepName = cmd.StepName;
        step.ApproverType = cmd.ApproverType;
        step.ApproverId = cmd.ApproverType == WorkflowApproverType.OrgUnitManager ? null : cmd.ApproverId;

        await unitOfWork.SaveChangesAsync(ct);
        return Unit.Value;
    }
}
