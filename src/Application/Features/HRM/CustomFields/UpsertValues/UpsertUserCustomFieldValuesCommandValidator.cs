namespace Application;

public sealed class UpsertUserCustomFieldValuesCommandValidator : AbstractValidator<UpsertUserCustomFieldValuesCommand>
{
    public UpsertUserCustomFieldValuesCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Values).NotNull();
        RuleForEach(x => x.Values).ChildRules(v =>
        {
            v.RuleFor(x => x.DefinitionId).NotEmpty();
            v.RuleFor(x => x.Value).NotNull().MaximumLength(2000);
        });
    }
}
