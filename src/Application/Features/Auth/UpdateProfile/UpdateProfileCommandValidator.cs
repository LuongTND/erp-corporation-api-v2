namespace Application;

public sealed class UpdateProfileCommandValidator : AbstractValidator<UpdateProfileCommand>
{
    public UpdateProfileCommandValidator()
    {
        When(x => x.FullName is not null, () =>
            RuleFor(x => x.FullName).NotEmpty().WithMessage(ValidationMessages.Required));

        When(x => x.Email is not null, () =>
            RuleFor(x => x.Email).EmailAddress().WithMessage(ValidationMessages.InvalidEmail));
    }
}
