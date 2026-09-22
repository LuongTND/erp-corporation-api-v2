namespace Application;

public sealed class GetJobPostingsQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetJobPostingsQuery, IEnumerable<JobPostingResponse>>
{
    public async Task<IEnumerable<JobPostingResponse>> Handle(GetJobPostingsQuery q, CancellationToken ct)
    {
        var postings = await (
            from p in unitOfWork.Repository<Domain.JobPosting>().Query()
            join u in unitOfWork.Repository<User>().Query() on p.AssignedToUserId equals u.Id into uj
            from u in uj.DefaultIfEmpty()
            where p.RecruitmentRequestId == q.RecruitmentRequestId
            orderby p.CreatedAt descending
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
        ).ToListAsync(ct);

        return postings;
    }
}
