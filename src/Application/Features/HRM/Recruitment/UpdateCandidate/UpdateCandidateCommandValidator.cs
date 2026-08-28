namespace Application;

public sealed class UpdateCandidateCommandValidator
    : AbstractValidator<UpdateCandidateCommand>
{
    public UpdateCandidateCommandValidator()
    {
        RuleFor(x => x.FullName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Email).EmailAddress().When(x => x.Email is not null);
        RuleFor(x => x.Phone).MaximumLength(20).When(x => x.Phone is not null);
        RuleFor(x => x.SourceChannel).NotEmpty().MaximumLength(100);
    }
}
