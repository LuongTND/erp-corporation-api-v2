namespace Application;

public sealed class CreateCustomFieldDefinitionCommandValidator : AbstractValidator<CreateCustomFieldDefinitionCommand>
{
    public CreateCustomFieldDefinitionCommandValidator()
    {
        RuleFor(x => x.Code).NotEmpty().MaximumLength(50)
            .Matches(@"^[A-Za-z0-9_]+$").WithMessage("Code chỉ được chứa chữ, số và dấu _");
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.FieldType).IsInEnum();
        RuleFor(x => x.Module).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Placeholder).MaximumLength(200).When(x => x.Placeholder != null);
        RuleFor(x => x.HelpText).MaximumLength(500).When(x => x.HelpText != null);
        RuleFor(x => x.Group).MaximumLength(100).When(x => x.Group != null);

        // Options required only for Select/MultiSelect
        RuleFor(x => x.Options)
            .NotEmpty()
            .When(x => x.FieldType is CustomFieldType.Select or CustomFieldType.MultiSelect)
            .WithMessage("Select/MultiSelect phải có ít nhất 1 option.");

        RuleForEach(x => x.Options).ChildRules(o =>
        {
            o.RuleFor(x => x.Value).NotEmpty().MaximumLength(100);
            o.RuleFor(x => x.Label).NotEmpty().MaximumLength(200);
        }).When(x => x.Options != null);
    }
}
