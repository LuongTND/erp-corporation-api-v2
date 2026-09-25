namespace Application;

public sealed class GetJobPostingDetailQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetJobPostingDetailQuery, JobPostingResponse>
{
    public async Task<JobPostingResponse> Handle(GetJobPostingDetailQuery q, CancellationToken ct)
    {
        var result = await (
            from p in unitOfWork.Repository<Domain.JobPosting>().Query()
            join u in unitOfWork.Repository<User>().Query() on p.AssignedToUserId equals u.Id into uj
            from u in uj.DefaultIfEmpty()
            where p.Id == q.PostingId
            select new JobPostingResponse
            {
                Id = p.Id,
                RecruitmentRequestId = p.RecruitmentRequestId,
                Title = p.Title,
                Channel = p.Channel.ToString(),
                WorkLocation = p.WorkLocation,
                PostUrl = p.PostUrl,
                Requirements = p.Requirements,
                Status = p.Status.ToString(),
                AssignedToUserId = p.AssignedToUserId,
                AssignedToName = u != null ? u.FullName : null,
                PostedAt = p.PostedAt,
                ExpiresAt = p.ExpiresAt,
                ApplicationCount = p.Applications.Count,
            }
        ).FirstOrDefaultAsync(ct)
            ?? throw new NotFoundException(ExceptionMessages.NotFound("JobPosting", q.PostingId));

        return result;
    }
}
