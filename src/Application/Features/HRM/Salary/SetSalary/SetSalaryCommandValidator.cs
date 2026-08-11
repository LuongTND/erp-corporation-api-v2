namespace Application;

public sealed class SetSalaryCommandValidator : AbstractValidator<SetSalaryCommand>
{
    public SetSalaryCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.HourlyRate).GreaterThan(0).WithMessage(ValidationMessages.GreaterThan);
        RuleFor(x => x.EffectiveFrom).NotEmpty();
        RuleFor(x => x.Reason).MaximumLength(500);
    }
}
