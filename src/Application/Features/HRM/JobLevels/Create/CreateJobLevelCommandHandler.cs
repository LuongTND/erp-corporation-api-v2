namespace Application;

public sealed class CreateJobLevelCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<CreateJobLevelCommand, Guid>
{
    public async Task<Guid> Handle(CreateJobLevelCommand cmd, CancellationToken ct)
    {
        var nameExists = await unitOfWork.Repository<JobLevel>()
            .AnyAsync(j => j.LevelName == cmd.LevelName, ct);
        if (nameExists)
            throw new ConflictException(ExceptionMessages.AlreadyExists("LevelName", cmd.LevelName));

        var jobLevel = new JobLevel
        {
            Id = Guid.NewGuid(),
            LevelName = cmd.LevelName,
            LevelOrder = cmd.LevelOrder,
            DefaultScopeType = cmd.DefaultScopeType,
            Description = cmd.Description,
            BaseSalaryMin = cmd.BaseSalaryMin,
            BaseSalaryMax = cmd.BaseSalaryMax
        };

        await unitOfWork.Repository<JobLevel>().AddAsync(jobLevel);
        await unitOfWork.EnsureSaveAsync(ct);
        return jobLevel.Id;
    }
}
