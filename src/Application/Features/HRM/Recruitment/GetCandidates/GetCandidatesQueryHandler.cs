namespace Application;

public sealed class GetCandidatesQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetCandidatesQuery, QueryResult<CandidateResponse>>
{
    public async Task<QueryResult<CandidateResponse>> Handle(GetCandidatesQuery q, CancellationToken ct)
    {
        var result = await unitOfWork.Repository<Candidate>()
            .GetPagedAsync(
                q.QueryInfo,
                filter: c =>
                    (!q.RecruitmentRequestId.HasValue || c.RecruitmentRequestId == q.RecruitmentRequestId.Value) &&
                    (!q.Stage.HasValue || c.Stage == q.Stage.Value),
                ct: ct);

        return new QueryResult<CandidateResponse>
        {
            TotalCount = result.TotalCount,
            Items = result.Items.Select(c => new CandidateResponse
            {
                Id = c.Id,
                RecruitmentRequestId = c.RecruitmentRequestId,
                FullName = c.FullName,
                Email = c.Email,
                Phone = c.Phone,
                CvUrl = c.CvUrl,
                SourceChannel = c.SourceChannel.ToString(),
                Stage = c.Stage.ToString(),
                RejectionReason = c.RejectionReason,
                CreatedAt = c.CreatedAt
            })
        };
    }
}
