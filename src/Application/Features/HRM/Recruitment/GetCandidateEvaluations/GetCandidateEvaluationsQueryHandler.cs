namespace Application;

public sealed class GetCandidateEvaluationsQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetCandidateEvaluationsQuery, QueryResult<CandidateEvaluationResponse>>
{
    public async Task<QueryResult<CandidateEvaluationResponse>> Handle(GetCandidateEvaluationsQuery q, CancellationToken ct)
    {
        var queryInfo = new QueryInfo { Top = q.PageSize, Skip = (q.Page - 1) * q.PageSize, NeedTotalCount = true };
        var result = await unitOfWork.Repository<ApplicationEvaluation>()
            .GetPagedAsync(queryInfo, filter: e => e.ApplicationId == q.ApplicationId, ct: ct);

        return new QueryResult<CandidateEvaluationResponse>
        {
            TotalCount = result.TotalCount,
            Items = result.Items.Select(e => new CandidateEvaluationResponse
            {
                Id = e.Id,
                ApplicationId = e.ApplicationId,
                EvaluatorId = e.EvaluatorId,
                Score = e.Score,
                StrengthNotes = e.StrengthNotes,
                WeaknessNotes = e.WeaknessNotes,
                Recommendation = e.Recommendation.ToString(),
                CreatedAt = e.CreatedAt
            })
        };
    }
}
