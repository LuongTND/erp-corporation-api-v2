namespace Application;

public sealed class GetCandidateDetailQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetCandidateDetailQuery, CandidateDetailResponse>
{
    public async Task<CandidateDetailResponse> Handle(GetCandidateDetailQuery q, CancellationToken ct)
    {
        var application = await unitOfWork.Repository<Domain.Application>()
            .FindAsync(x => x.Id == q.ApplicationId, ct)
            ?? throw new NotFoundException(ExceptionMessages.NotFound("Application", q.ApplicationId));

        var applicant = await unitOfWork.Repository<Applicant>()
            .FindAsync(a => a.Id == application.ApplicantId, ct)
            ?? throw new NotFoundException(ExceptionMessages.NotFound("Applicant", application.ApplicantId));

        var evaluations = await unitOfWork.Repository<ApplicationEvaluation>()
            .GetPagedAsync(new QueryInfo { Top = 100, Skip = 0, NeedTotalCount = false },
                filter: e => e.ApplicationId == application.Id, ct: ct);

        return new CandidateDetailResponse
        {
            Id = application.Id,
            ApplicantId = applicant.Id,
            RecruitmentRequestId = application.RecruitmentRequestId,
            FullName = applicant.FullName,
            Email = applicant.Email,
            Phone = applicant.Phone,
            SourceChannel = application.SourceChannel.ToString(),
            Stage = application.Stage.ToString(),
            RejectionReason = application.RejectionReason,
            Notes = applicant.Notes,
            ConvertedEmployeeId = application.ConvertedEmployeeId,
            CreatedAt = application.CreatedAt,
            Evaluations = evaluations.Items.Select(e => new CandidateEvaluationResponse
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
