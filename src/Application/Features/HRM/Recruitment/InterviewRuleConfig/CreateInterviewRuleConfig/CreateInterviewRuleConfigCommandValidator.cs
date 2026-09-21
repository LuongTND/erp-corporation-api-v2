namespace Application;

public sealed class CreateInterviewRuleConfigCommandValidator
    : AbstractValidator<CreateInterviewRuleConfigCommand>
{
    public CreateInterviewRuleConfigCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Context).IsInEnum();
        RuleFor(x => x.Priority).GreaterThanOrEqualTo(0);
        RuleFor(x => x.NotifyRoleKey).MaximumLength(100).When(x => x.NotifyRoleKey is not null);
        RuleFor(x => x.Steps).NotNull();
        RuleForEach(x => x.Steps).SetValidator(new InterviewRuleConfigStepCommandValidator());
    }
}

public sealed class InterviewRuleConfigStepCommandValidator
    : AbstractValidator<CreateInterviewRuleConfigStepCommand>
{
    public InterviewRuleConfigStepCommandValidator()
    {
        RuleFor(x => x.RoundNumber).GreaterThan(0);
        RuleFor(x => x.RoundTypeId).NotEmpty();
        RuleFor(x => x.InterviewerRoleKey).NotEmpty().MaximumLength(100);
        RuleFor(x => x.SchedulerRoleKey).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Location).IsInEnum();
        RuleFor(x => x.Name).MaximumLength(200).When(x => x.Name is not null);
    }
}
