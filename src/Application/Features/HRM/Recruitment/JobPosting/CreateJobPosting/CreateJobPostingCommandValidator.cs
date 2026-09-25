namespace Application;

public sealed class CreateJobPostingCommandValidator : AbstractValidator<CreateJobPostingCommand>
{
    public CreateJobPostingCommandValidator()
    {
        RuleFor(x => x.RecruitmentRequestId).NotEmpty();
        RuleFor(x => x.PostUrl).MaximumLength(500);
        RuleFor(x => x.Requirements).MaximumLength(2000);
    }
}
