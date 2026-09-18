namespace Application;

public sealed class CreateInterviewRuleConfigCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<CreateInterviewRuleConfigCommand, Guid>
{
    public async Task<Guid> Handle(CreateInterviewRuleConfigCommand cmd, CancellationToken ct)
    {
        var config = new Domain.InterviewRuleConfig
        {
            Id = Guid.NewGuid(),
            Name = cmd.Name,
            Context = cmd.Context,
            RegionId = cmd.RegionId,
            DepartmentId = cmd.DepartmentId,
            NotifyRoleKey = cmd.NotifyRoleKey,
            Priority = cmd.Priority,
            IsActive = true
        };
        await unitOfWork.Repository<Domain.InterviewRuleConfig>().AddAsync(config);

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
        return config.Id;
    }
}
