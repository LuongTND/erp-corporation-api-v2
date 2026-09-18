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

        // Batch-load request codes
        var requestIds = result.Items
            .Where(c => c.RecruitmentRequestId.HasValue)
            .Select(c => c.RecruitmentRequestId!.Value)
            .Distinct()
            .ToList();

        var requestCodeMap = requestIds.Count > 0
            ? (await unitOfWork.Repository<RecruitmentRequest>()
                .GetAllAsync(r => requestIds.Contains(r.Id), ct))
                .ToDictionary(r => r.Id, r => r.RequestCode)
            : new Dictionary<Guid, string>();

        // Batch-load latest evaluation per candidate
        var candidateIds = result.Items.Select(c => c.Id).ToList();
        var evaluations = candidateIds.Count > 0
            ? await unitOfWork.Repository<CandidateEvaluation>()
                .GetAllAsync(e => candidateIds.Contains(e.CandidateId), ct)
            : [];

        var latestEvalMap = evaluations
            .GroupBy(e => e.CandidateId)
            .ToDictionary(g => g.Key, g => g.OrderByDescending(e => e.CreatedAt).First());

        return new QueryResult<CandidateResponse>
        {
            TotalCount = result.TotalCount,
            Items = result.Items.Select(c =>
            {
                latestEvalMap.TryGetValue(c.Id, out var eval);
                requestCodeMap.TryGetValue(c.RecruitmentRequestId ?? Guid.Empty, out var code);
                return new CandidateResponse
                {
                    Id = c.Id,
                    RecruitmentRequestId = c.RecruitmentRequestId,
                    RequestCode = code,
                    FullName = c.FullName,
                    Email = c.Email,
                    Phone = c.Phone,
                    CvUrl = c.CvUrl,
                    SourceChannel = c.SourceChannel.ToString(),
                    Stage = c.Stage.ToString(),
                    RejectionReason = c.RejectionReason,
                    EvaluationScore = eval?.Score,
                    EvaluationRecommendation = eval?.Recommendation.ToString(),
                    CreatedAt = c.CreatedAt
                };
            })
        };
    }
}
