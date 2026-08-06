namespace Application;

public sealed class UpdateEmployeeCommandValidator : AbstractValidator<UpdateEmployeeCommand>
{
    public UpdateEmployeeCommandValidator()
    {
        RuleFor(x => x.FullName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.JobLevelId).NotEmpty();
        RuleFor(x => x.PhoneNumber).MaximumLength(20).When(x => x.PhoneNumber is not null);
        RuleFor(x => x.IdentityCardNumber).MaximumLength(20).When(x => x.IdentityCardNumber is not null);
        RuleFor(x => x.TaxCode).MaximumLength(20).When(x => x.TaxCode is not null);
        RuleFor(x => x.BankAccountNumber).MaximumLength(30).When(x => x.BankAccountNumber is not null);
    }
}
