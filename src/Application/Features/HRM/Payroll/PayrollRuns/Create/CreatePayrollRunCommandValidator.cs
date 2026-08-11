namespace Application;

public class CreatePayrollRunCommandValidator : AbstractValidator<CreatePayrollRunCommand>
{
    public CreatePayrollRunCommandValidator()
    {
        RuleFor(x => x.Month).InclusiveBetween(1, 12);
        RuleFor(x => x.Year).InclusiveBetween(2020, 2100);
        RuleFor(x => x.Note).MaximumLength(1000).When(x => x.Note is not null);
    }
}
