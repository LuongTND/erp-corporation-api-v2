namespace Application;

public sealed class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
{
    public ChangePasswordCommandValidator()
    {
        RuleFor(x => x.CurrentPassword).NotEmpty().WithMessage(ValidationMessages.Required);

        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage(ValidationMessages.Required)
            .MinimumLength(8).WithMessage(ValidationMessages.MinLength)
            .Matches(@"[A-Z]").WithMessage("{PropertyName} must contain at least one uppercase letter")
            .Matches(@"[a-z]").WithMessage("{PropertyName} must contain at least one lowercase letter")
            .Matches(@"[0-9]").WithMessage("{PropertyName} must contain at least one digit")
            .Matches(@"[!@#$%^&*()_+\-=\[\]{};':""\\|,.<>\/?]").WithMessage("{PropertyName} must contain at least one special character")
            .NotEqual(x => x.CurrentPassword).WithMessage(ValidationMessages.MustNotMatch);
    }
}
