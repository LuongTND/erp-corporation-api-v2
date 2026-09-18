namespace Application;

public sealed class GetCandidatesQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetCandidatesQuery, QueryResult<CandidateResponse>>
{
    public async Task<QueryResult<CandidateResponse>> Handle(GetCandidatesQuery q, CancellationToken ct)
    {
        var result = await unitOfWork.Repository<Domain.Application>()
            .GetPagedAsync(
                q.QueryInfo,
                filter: a =>
                    (!q.RecruitmentRequestId.HasValue || a.RecruitmentRequestId == q.RecruitmentRequestId.Value) &&
                    (!q.Stage.HasValue || a.Stage == q.Stage.Value),
                ct: ct);

        var applicantIds = result.Items.Select(a => a.ApplicantId).Distinct().ToList();
        var applicantMap = applicantIds.Count > 0
            ? (await unitOfWork.Repository<Applicant>().GetAllAsync(a => applicantIds.Contains(a.Id), ct))
                .ToDictionary(a => a.Id)
            : new Dictionary<Guid, Applicant>();

        var requestIds = result.Items
            .Where(a => a.RecruitmentRequestId.HasValue)
            .Select(a => a.RecruitmentRequestId!.Value)
            .Distinct().ToList();
        var requestCodeMap = requestIds.Count > 0
            ? (await unitOfWork.Repository<RecruitmentRequest>().GetAllAsync(r => requestIds.Contains(r.Id), ct))
                .ToDictionary(r => r.Id, r => r.RequestCode)
            : new Dictionary<Guid, string>();

        var appIds = result.Items.Select(a => a.Id).ToList();
        var evaluations = appIds.Count > 0
            ? await unitOfWork.Repository<ApplicationEvaluation>().GetAllAsync(e => appIds.Contains(e.ApplicationId), ct)
            : [];
        var latestEvalMap = evaluations
            .GroupBy(e => e.ApplicationId)
            .ToDictionary(g => g.Key, g => g.OrderByDescending(e => e.CreatedAt).First());

        return new QueryResult<CandidateResponse>
        {
            TotalCount = result.TotalCount,
            Items = result.Items.Select(a =>
            {
                applicantMap.TryGetValue(a.ApplicantId, out var applicant);
                latestEvalMap.TryGetValue(a.Id, out var eval);
                requestCodeMap.TryGetValue(a.RecruitmentRequestId ?? Guid.Empty, out var code);
                return new CandidateResponse
                {
                    Id = a.Id,
                    ApplicantId = a.ApplicantId,
                    RecruitmentRequestId = a.RecruitmentRequestId,
                    RequestCode = code,
                    FullName = applicant?.FullName ?? string.Empty,
                    Email = applicant?.Email,
                    Phone = applicant?.Phone,
                    SourceChannel = a.SourceChannel.ToString(),
                    Stage = a.Stage.ToString(),
                    RejectionReason = a.RejectionReason,
                    EvaluationScore = eval?.Score,
                    EvaluationRecommendation = eval?.Recommendation.ToString(),
                    CreatedAt = a.CreatedAt
                };
            })
        };
    }
}
