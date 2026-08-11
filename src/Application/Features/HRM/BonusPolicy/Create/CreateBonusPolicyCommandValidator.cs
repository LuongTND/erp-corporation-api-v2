namespace Application;

public sealed class CreateBonusPolicyCommandValidator : AbstractValidator<CreateBonusPolicyCommand>
{
    public CreateBonusPolicyCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage(ValidationMessages.Required).MaximumLength(255);
        RuleFor(x => x.Description).MaximumLength(1000);
    }
}
