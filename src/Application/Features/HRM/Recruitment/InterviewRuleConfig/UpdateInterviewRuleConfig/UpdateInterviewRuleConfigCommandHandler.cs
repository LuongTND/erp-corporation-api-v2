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
        config.NotifyRoleKey = cmd.NotifyRoleKey;
        config.Priority = cmd.Priority;
        config.IsActive = cmd.IsActive;

        // Replace steps: delete existing, add new
        var existing = await unitOfWork.Repository<InterviewRuleConfigStep>()
            .GetAllTrackedAsync(s => s.InterviewRuleConfigId == config.Id, ct);
        await unitOfWork.Repository<InterviewRuleConfigStep>().RemoveRangeAsync(existing);

        foreach (var s in cmd.Steps)
        {
            await unitOfWork.Repository<InterviewRuleConfigStep>().AddAsync(new InterviewRuleConfigStep
            {
                Id = Guid.NewGuid(),
                InterviewRuleConfigId = config.Id,
                RoundNumber = s.RoundNumber,
                Label = s.Label,
                InterviewerRoleKey = s.InterviewerRoleKey,
                SchedulerRoleKey = s.SchedulerRoleKey,
                Location = s.Location
            });
        }

        await unitOfWork.EnsureSaveAsync(ct);
        return Unit.Value;
    }
}
