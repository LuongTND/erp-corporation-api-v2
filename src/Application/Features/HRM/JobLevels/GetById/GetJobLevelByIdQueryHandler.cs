namespace Application;

public sealed class GetJobLevelByIdQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetJobLevelByIdQuery, JobLevelResponse>
{
    public async Task<JobLevelResponse> Handle(GetJobLevelByIdQuery query, CancellationToken ct)
    {
        var jobLevel = await unitOfWork.Repository<JobLevel>()
            .FindAsync(j => j.Id == query.JobLevelId, ct)
            ?? throw new NotFoundException(ExceptionMessages.NotFound("JobLevel", query.JobLevelId));

        return new JobLevelResponse
        {
            Id = jobLevel.Id,
            LevelName = jobLevel.LevelName,
            LevelOrder = jobLevel.LevelOrder,
            DefaultScopeType = jobLevel.DefaultScopeType.ToString(),
            Description = jobLevel.Description,
            IsDeleted = jobLevel.IsDeleted
        };
    }
}
