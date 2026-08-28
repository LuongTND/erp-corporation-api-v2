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
            InterviewerRoleKey = cmd.InterviewerRoleKey,
            Location = cmd.Location,
            SchedulerRoleKey = cmd.SchedulerRoleKey,
            NotifyRoleKey = cmd.NotifyRoleKey,
            Priority = cmd.Priority,
            IsActive = true,
        };
        await unitOfWork.Repository<Domain.InterviewRuleConfig>().AddAsync(config);
        await unitOfWork.EnsureSaveAsync(ct);
        return config.Id;
    }
}
