namespace Application;

public sealed class UpdateInterviewRuleConfigCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateInterviewRuleConfigCommand, Unit>
{
    public async Task<Unit> Handle(UpdateInterviewRuleConfigCommand cmd, CancellationToken ct)
    {
        var config = await unitOfWork.Repository<Domain.InterviewRuleConfig>()
            .FindAsync(r => r.Id == cmd.Id, ct)
            ?? throw new NotFoundException(ExceptionMessages.NotFound("InterviewRuleConfig", cmd.Id));

        config.Name = cmd.Name;
        config.InterviewerRoleKey = cmd.InterviewerRoleKey;
        config.Location = cmd.Location;
        config.SchedulerRoleKey = cmd.SchedulerRoleKey;
        config.NotifyRoleKey = cmd.NotifyRoleKey;
        config.Priority = cmd.Priority;
        config.IsActive = cmd.IsActive;

        await unitOfWork.EnsureSaveAsync(ct);
        return Unit.Value;
    }
}
