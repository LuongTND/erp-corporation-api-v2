namespace Application;

public sealed class GetJobTitleByIdQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetJobTitleByIdQuery, JobTitleResponse>
{
    public async Task<JobTitleResponse> Handle(GetJobTitleByIdQuery query, CancellationToken ct)
    {
        var jobTitle = await unitOfWork.Repository<JobTitle>()
            .FindAsync(j => j.Id == query.JobTitleId, ct)
            ?? throw new NotFoundException(ExceptionMessages.NotFound("JobTitle", query.JobTitleId));

        return new JobTitleResponse
        {
            Id = jobTitle.Id,
            Code = jobTitle.Code,
            Name = jobTitle.Name,
            Description = jobTitle.Description,
            Level = jobTitle.Level.ToString(),
            UnitType = jobTitle.UnitType.ToString(),
            IsDeleted = jobTitle.IsDeleted
        };
    }
}
