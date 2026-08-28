namespace Application;

public sealed class EvaluateCandidateCommandHandler(IUnitOfWork unitOfWork, IUserContext currentUser)
    : IRequestHandler<EvaluateCandidateCommand, Guid>
{
    public async Task<Guid> Handle(EvaluateCandidateCommand cmd, CancellationToken ct)
    {
        var candidate = await unitOfWork.Repository<Candidate>()
            .FindAsync(c => c.Id == cmd.CandidateId, ct)
            ?? throw new NotFoundException(ExceptionMessages.NotFound("Candidate", cmd.CandidateId));

        if (candidate.Stage is not (CandidateStage.StoreInterview or CandidateStage.ProductionInterview))
            throw new BadRequestException("Ứng viên phải ở giai đoạn phỏng vấn để đánh giá.");

        if (!Enum.TryParse<EvaluationRecommendation>(cmd.Recommendation, out var recommendation))
            throw new BadRequestException($"Recommendation không hợp lệ: {cmd.Recommendation}");

        var evaluation = new CandidateEvaluation
        {
            Id = Guid.NewGuid(),
            CandidateId = cmd.CandidateId,
            EvaluatorId = currentUser.UserId,
            IsStoreEvaluation = cmd.IsStoreEvaluation,
            Score = cmd.Score,
            StrengthNotes = cmd.StrengthNotes,
            WeaknessNotes = cmd.WeaknessNotes,
            Recommendation = recommendation
        };

        await unitOfWork.Repository<CandidateEvaluation>().AddAsync(evaluation);
        await unitOfWork.EnsureSaveAsync(ct);
        return evaluation.Id;
    }
}
