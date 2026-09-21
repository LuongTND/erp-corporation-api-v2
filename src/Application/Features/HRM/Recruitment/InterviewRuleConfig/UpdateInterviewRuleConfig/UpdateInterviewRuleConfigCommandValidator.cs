namespace Application;

public sealed class UpdateInterviewRuleConfigCommandValidator
    : AbstractValidator<UpdateInterviewRuleConfigCommand>
{
    public UpdateInterviewRuleConfigCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Priority).GreaterThanOrEqualTo(0);
        RuleFor(x => x.NotifyRoleKey).MaximumLength(100).When(x => x.NotifyRoleKey is not null);
        RuleFor(x => x.Steps).NotNull();
        RuleForEach(x => x.Steps).SetValidator(new InterviewRuleConfigStepCommandValidator());
    }
}
