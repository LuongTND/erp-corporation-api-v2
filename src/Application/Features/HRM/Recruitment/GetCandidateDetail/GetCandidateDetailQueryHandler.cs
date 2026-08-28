namespace Application;

public sealed class GetCandidateDetailQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetCandidateDetailQuery, CandidateDetailResponse>
{
    public async Task<CandidateDetailResponse> Handle(GetCandidateDetailQuery q, CancellationToken ct)
    {
        var c = await unitOfWork.Repository<Candidate>()
            .FindAsync(x => x.Id == q.CandidateId, ct)
            ?? throw new NotFoundException(ExceptionMessages.NotFound("Candidate", q.CandidateId));

        var evaluations = await unitOfWork.Repository<CandidateEvaluation>()
            .GetPagedAsync(new QueryInfo { Top = 100, Skip = 0, NeedTotalCount = false }, filter: e => e.CandidateId == c.Id, ct: ct);

        return new CandidateDetailResponse
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
            Notes = c.Notes,
            ConvertedEmployeeId = c.ConvertedEmployeeId,
            CreatedAt = c.CreatedAt,
            Evaluations = evaluations.Items.Select(e => new CandidateEvaluationResponse
            {
                Id = e.Id,
                CandidateId = e.CandidateId,
                EvaluatorId = e.EvaluatorId,
                IsStoreEvaluation = e.IsStoreEvaluation,
                Score = e.Score,
                StrengthNotes = e.StrengthNotes,
                WeaknessNotes = e.WeaknessNotes,
                Recommendation = e.Recommendation.ToString(),
                CreatedAt = e.CreatedAt
            })
        };
    }
}
