namespace Application;

public sealed class CreateContractCommandValidator : AbstractValidator<CreateContractCommand>
{
    public CreateContractCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Type).IsInEnum().WithMessage(ValidationMessages.InvalidEnumValue);
        RuleFor(x => x.StartDate).NotEmpty();
        RuleFor(x => x.EndDate)
            .GreaterThan(x => x.StartDate)
            .When(x => x.EndDate.HasValue)
            .WithMessage("EndDate phải sau StartDate");
        RuleFor(x => x.Salary).GreaterThan(0).WithMessage(ValidationMessages.GreaterThan);
        RuleFor(x => x.SalaryForSocialInsurance)
            .GreaterThan(0)
            .When(x => x.SalaryForSocialInsurance.HasValue)
            .WithMessage(ValidationMessages.GreaterThan);
        RuleFor(x => x.PositionTitle).MaximumLength(200);
        RuleFor(x => x.OriginalFileName).NotEmpty();
    }
}
