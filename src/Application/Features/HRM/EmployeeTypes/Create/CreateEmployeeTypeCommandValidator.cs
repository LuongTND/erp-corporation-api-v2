namespace Application;

public sealed class CreateEmployeeTypeCommandValidator : AbstractValidator<CreateEmployeeTypeCommand>
{
    public CreateEmployeeTypeCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Code).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Description).MaximumLength(500).When(x => x.Description is not null);
    }
}
