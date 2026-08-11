namespace Application;

public class UpsertKpiEntryCommandValidator : AbstractValidator<UpsertKpiEntryCommand>
{
    public UpsertKpiEntryCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.KpiMetricId).NotEmpty();
        RuleFor(x => x.Month).InclusiveBetween(1, 12);
        RuleFor(x => x.Year).InclusiveBetween(2020, 2100);
        RuleFor(x => x.ActualValue).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Score).InclusiveBetween(0, 100);
        RuleFor(x => x.Note).MaximumLength(500).When(x => x.Note is not null);
    }
}
