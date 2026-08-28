namespace Application;

public sealed class GetRecruitmentRequestDetailQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetRecruitmentRequestDetailQuery, RecruitmentRequestDetailResponse>
{
    public async Task<RecruitmentRequestDetailResponse> Handle(GetRecruitmentRequestDetailQuery q, CancellationToken ct)
    {
        var r = await unitOfWork.Repository<RecruitmentRequest>()
            .FindAsync(x => x.Id == q.RequestId, ct)
            ?? throw new NotFoundException(ExceptionMessages.NotFound("RecruitmentRequest", q.RequestId));

        var postings = await unitOfWork.Repository<JobPosting>()
            .GetPagedAsync(new QueryInfo { Top = 100, Skip = 0, NeedTotalCount = false }, filter: p => p.RecruitmentRequestId == r.Id, ct: ct);

        return new RecruitmentRequestDetailResponse
        {
            Id = r.Id,
            RequestContext = r.RequestContext.ToString(),
            DepartmentId = r.DepartmentId,
            StoreId = r.StoreId,
            PositionTitle = r.PositionTitle,
            RequestedByUserId = r.RequestedByUserId,
            Headcount = r.Headcount,
            Reason = r.Reason,
            JobDescription = r.JobDescription,
            RequiredByDate = r.RequiredByDate,
            Status = r.Status.ToString(),
            RejectionNote = r.RejectionNote,
            NeedMoreInfoNote = r.NeedMoreInfoNote,
            CreatedAt = r.CreatedAt,
            JobPostings = postings.Items.Select(p => new JobPostingResponse
            {
                Id = p.Id,
                RecruitmentRequestId = p.RecruitmentRequestId,
                Title = p.Title,
                Channel = p.Channel.ToString(),
                PostUrl = p.PostUrl,
                EstimatedCost = p.EstimatedCost,
                CostStatus = p.CostStatus.ToString(),
                CostApprovedByUserId = p.CostApprovedByUserId,
                CostApprovedAt = p.CostApprovedAt,
                CostRejectionNote = p.CostRejectionNote,
                PostedAt = p.PostedAt,
                ExpiresAt = p.ExpiresAt,
                CreatedAt = p.CreatedAt
            })
        };
    }
}
