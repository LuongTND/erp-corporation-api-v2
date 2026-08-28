namespace Application;

public sealed class RejectJobPostingCostCommandValidator
    : AbstractValidator<RejectJobPostingCostCommand>
{
    public RejectJobPostingCostCommandValidator()
    {
        RuleFor(x => x.RejectionNote).NotEmpty().MaximumLength(2000);
    }
}
