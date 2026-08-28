namespace Application;

public sealed class RejectRecruitmentRequestCommandValidator
    : AbstractValidator<RejectRecruitmentRequestCommand>
{
    public RejectRecruitmentRequestCommandValidator()
    {
        RuleFor(x => x.RejectionNote).NotEmpty().MaximumLength(2000);
    }
}
