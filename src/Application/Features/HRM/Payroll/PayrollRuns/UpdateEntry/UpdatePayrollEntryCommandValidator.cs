namespace Application;

public class UpdatePayrollEntryCommandValidator : AbstractValidator<UpdatePayrollEntryCommand>
{
    public UpdatePayrollEntryCommandValidator()
    {
        RuleFor(x => x.EntryId).NotEmpty();
        RuleFor(x => x.HoursWorked).GreaterThanOrEqualTo(0);
        RuleFor(x => x.BonusAmount).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Note).MaximumLength(500).When(x => x.Note is not null);
    }
}
