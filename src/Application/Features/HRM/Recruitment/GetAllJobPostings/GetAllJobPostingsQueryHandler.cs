namespace Application;

public sealed class GetAllJobPostingsQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetAllJobPostingsQuery, QueryResult<JobPostingResponse>>
{
    public async Task<QueryResult<JobPostingResponse>> Handle(GetAllJobPostingsQuery q, CancellationToken ct)
    {
        var result = await unitOfWork.Repository<JobPosting>()
            .GetPagedAsync(
                q.QueryInfo,
                filter: p =>
                    (!q.CostStatus.HasValue || p.CostStatus == q.CostStatus.Value) &&
                    (!q.RecruitmentRequestId.HasValue || p.RecruitmentRequestId == q.RecruitmentRequestId.Value),
                ct: ct);

        // Batch-load request codes
        var requestIds = result.Items.Select(p => p.RecruitmentRequestId).Distinct().ToList();
        var requestCodeMap = (await unitOfWork.Repository<RecruitmentRequest>()
                .GetAllAsync(r => requestIds.Contains(r.Id), ct))
                .ToDictionary(r => r.Id, r => r.RequestCode);

        return new QueryResult<JobPostingResponse>
        {
            TotalCount = result.TotalCount,
            Items = result.Items.Select(p =>
            {
                requestCodeMap.TryGetValue(p.RecruitmentRequestId, out var code);
                return new JobPostingResponse
                {
                    Id = p.Id,
                    RecruitmentRequestId = p.RecruitmentRequestId,
                    RequestCode = code,
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
                };
            })
        };
    }
}
