namespace Application;

public sealed class GetJobPostingsQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetJobPostingsQuery, QueryResult<JobPostingResponse>>
{
    public async Task<QueryResult<JobPostingResponse>> Handle(GetJobPostingsQuery q, CancellationToken ct)
    {
        var queryInfo = new QueryInfo { Top = q.PageSize, Skip = (q.Page - 1) * q.PageSize, NeedTotalCount = true };
        var result = await unitOfWork.Repository<JobPosting>()
            .GetPagedAsync(queryInfo, filter: p => p.RecruitmentRequestId == q.RecruitmentRequestId, ct: ct);

        var recruitmentRequest = await unitOfWork.Repository<RecruitmentRequest>()
            .FindAsync(r => r.Id == q.RecruitmentRequestId, ct);

        return new QueryResult<JobPostingResponse>
        {
            TotalCount = result.TotalCount,
            Items = result.Items.Select(p => new JobPostingResponse
            {
                Id = p.Id,
                RecruitmentRequestId = p.RecruitmentRequestId,
                RequestCode = recruitmentRequest?.RequestCode,
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
