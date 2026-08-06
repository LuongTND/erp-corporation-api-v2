namespace Application;

public sealed class UpdateCustomFieldDefinitionCommandValidator : AbstractValidator<UpdateCustomFieldDefinitionCommand>
{
    public UpdateCustomFieldDefinitionCommandValidator()
    {
        RuleFor(x => x.DefinitionId).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Placeholder).MaximumLength(200).When(x => x.Placeholder != null);
        RuleFor(x => x.HelpText).MaximumLength(500).When(x => x.HelpText != null);
        RuleFor(x => x.Group).MaximumLength(100).When(x => x.Group != null);

        RuleForEach(x => x.Options).ChildRules(o =>
        {
            o.RuleFor(x => x.Value).NotEmpty().MaximumLength(100);
            o.RuleFor(x => x.Label).NotEmpty().MaximumLength(200);
        }).When(x => x.Options != null);
    }
}
