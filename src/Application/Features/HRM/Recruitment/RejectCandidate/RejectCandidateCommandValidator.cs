namespace Application;

public sealed class RejectCandidateCommandValidator
    : AbstractValidator<RejectCandidateCommand>
{
    public RejectCandidateCommandValidator()
    {
        RuleFor(x => x.RejectionReason).NotEmpty().MaximumLength(2000);
    }
}
