namespace Application;

public sealed class DeleteWorkflowStepCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<DeleteWorkflowStepCommand, Unit>
{
    public async Task<Unit> Handle(DeleteWorkflowStepCommand cmd, CancellationToken ct)
    {
        var step = await unitOfWork.Repository<WorkflowTemplateStep>()
            .FindAsync(s => s.Id == cmd.StepId && s.TemplateId == cmd.TemplateId, ct)
            ?? throw new NotFoundException(ExceptionMessages.NotFound("WorkflowTemplateStep", cmd.StepId));

        await unitOfWork.Repository<WorkflowTemplateStep>().RemoveAsync(step);
        await unitOfWork.EnsureSaveAsync(ct);
        return Unit.Value;
    }
}
