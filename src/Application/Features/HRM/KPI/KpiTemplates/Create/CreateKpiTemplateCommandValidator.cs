namespace Application;

public sealed class CreateKpiTemplateCommandValidator : AbstractValidator<CreateKpiTemplateCommand>
{
    public CreateKpiTemplateCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage(ValidationMessages.Required).MaximumLength(255);
        RuleFor(x => x.DepartmentId).NotEmpty().WithMessage(ValidationMessages.Required);
        RuleFor(x => x.Metrics).NotEmpty().WithMessage(ValidationMessages.ListNotEmpty);
        RuleForEach(x => x.Metrics).ChildRules(m =>
        {
            m.RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
            m.RuleFor(x => x.Unit).NotEmpty().MaximumLength(50);
            m.RuleFor(x => x.Weight).GreaterThan(0).WithMessage(ValidationMessages.GreaterThan);
            m.RuleFor(x => x.Target).GreaterThanOrEqualTo(0);
        });
        RuleFor(x => x.Metrics)
            .Must(metrics => Math.Abs(metrics.Sum(m => m.Weight) - 1m) < 0.001m)
            .WithMessage("Tổng Weight của tất cả metrics phải bằng 1.0 (100%).");
    }
}
