namespace Application;

public sealed class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage(ValidationMessages.Required)
            .EmailAddress().WithMessage(ValidationMessages.InvalidEmail)
            .MaximumLength(255).WithMessage(ValidationMessages.MaxLength);

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage(ValidationMessages.Required)
            .MinimumLength(8).WithMessage(ValidationMessages.MinLength);
    }
}
