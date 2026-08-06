namespace Application;

public sealed class CreateEmployeeCommandValidator : AbstractValidator<CreateEmployeeCommand>
{
    public CreateEmployeeCommandValidator()
    {
        RuleFor(x => x.FullName).NotEmpty().MaximumLength(255);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(255);
        RuleFor(x => x.JobLevelId).NotEmpty();
        RuleFor(x => x.DateOfJoin).NotEmpty();
        RuleFor(x => x.EmployeeCode).MaximumLength(50).When(x => x.EmployeeCode != null);
        RuleFor(x => x.PhoneNumber).MaximumLength(20).When(x => x.PhoneNumber != null);
        RuleFor(x => x.IdentityCardNumber).MaximumLength(20).When(x => x.IdentityCardNumber != null);
        RuleFor(x => x.TaxCode).MaximumLength(20).When(x => x.TaxCode != null);
        RuleFor(x => x.SocialInsuranceCode).MaximumLength(20).When(x => x.SocialInsuranceCode != null);
    }
}
