namespace Application;

public sealed class EvaluateCandidateCommandValidator
    : AbstractValidator<EvaluateCandidateCommand>
{
    public EvaluateCandidateCommandValidator()
    {
        RuleFor(x => x.Score).InclusiveBetween(1, 10);
        RuleFor(x => x.Recommendation).NotEmpty()
            .Must(r => Enum.TryParse<EvaluationRecommendation>(r, out _))
            .WithMessage("Recommendation phải là Hire, Reject hoặc NeedMoreInterview.");
    }
}
