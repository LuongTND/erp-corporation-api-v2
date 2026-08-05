namespace Application;

public sealed class CreateJobLevelCommandValidator : AbstractValidator<CreateJobLevelCommand>
{
    public CreateJobLevelCommandValidator()
    {
        RuleFor(x => x.LevelName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.LevelOrder).GreaterThan(0);
        RuleFor(x => x.DefaultScopeType).IsInEnum();
        RuleFor(x => x.Description).MaximumLength(500).When(x => x.Description is not null);
        RuleFor(x => x.BaseSalaryMax)
            .GreaterThanOrEqualTo(x => x.BaseSalaryMin)
            .When(x => x.BaseSalaryMin.HasValue && x.BaseSalaryMax.HasValue)
            .WithMessage("BaseSalaryMax phải >= BaseSalaryMin.");
    }
}
