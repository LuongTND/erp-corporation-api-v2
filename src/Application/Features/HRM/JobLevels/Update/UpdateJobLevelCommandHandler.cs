namespace Application;

public sealed class UpdateJobLevelCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateJobLevelCommand, Unit>
{
    public async Task<Unit> Handle(UpdateJobLevelCommand cmd, CancellationToken ct)
    {
        var jobLevel = await unitOfWork.Repository<JobLevel>()
            .FindTrackedAsync(j => j.Id == cmd.JobLevelId, ct)
            ?? throw new NotFoundException(ExceptionMessages.NotFound("JobLevel", cmd.JobLevelId));

        var nameExists = await unitOfWork.Repository<JobLevel>()
            .AnyAsync(j => j.LevelName == cmd.LevelName && j.Id != cmd.JobLevelId, ct);
        if (nameExists)
            throw new ConflictException(ExceptionMessages.AlreadyExists("LevelName", cmd.LevelName));

        jobLevel.LevelName = cmd.LevelName;
        jobLevel.LevelOrder = cmd.LevelOrder;
        jobLevel.DefaultScopeType = cmd.DefaultScopeType;
        jobLevel.Description = cmd.Description;
        jobLevel.BaseSalaryMin = cmd.BaseSalaryMin;
        jobLevel.BaseSalaryMax = cmd.BaseSalaryMax;

        await unitOfWork.EnsureSaveAsync(ct);
        return Unit.Value;
    }
}
