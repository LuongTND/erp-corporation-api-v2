namespace Application;

public sealed class EvaluateCandidateCommandHandler(IUnitOfWork unitOfWork, IUserContext currentUser)
    : IRequestHandler<EvaluateCandidateCommand, Guid>
{
    public async Task<Guid> Handle(EvaluateCandidateCommand cmd, CancellationToken ct)
    {
        var application = await unitOfWork.Repository<Domain.Application>()
            .FindAsync(a => a.Id == cmd.ApplicationId, ct)
            ?? throw new NotFoundException(ExceptionMessages.NotFound("Application", cmd.ApplicationId));

        if (application.Stage != ApplicationStage.Interview)
            throw new BadRequestException("Ứng viên phải ở giai đoạn phỏng vấn để đánh giá.");

        if (!Enum.TryParse<EvaluationRecommendation>(cmd.Recommendation, out var recommendation))
            throw new BadRequestException($"Recommendation không hợp lệ: {cmd.Recommendation}");

        var evaluation = new ApplicationEvaluation
        {
            Id = Guid.NewGuid(),
            ApplicationId = cmd.ApplicationId,
            EvaluatorId = currentUser.UserId,
            Score = cmd.Score,
            StrengthNotes = cmd.StrengthNotes,
            WeaknessNotes = cmd.WeaknessNotes,
            Recommendation = recommendation
        };

        await unitOfWork.Repository<ApplicationEvaluation>().AddAsync(evaluation);
        await unitOfWork.EnsureSaveAsync(ct);
        return evaluation.Id;
    }
}
