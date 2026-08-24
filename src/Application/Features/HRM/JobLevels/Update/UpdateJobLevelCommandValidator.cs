namespace Application;

public sealed class UpdateJobLevelCommandValidator : AbstractValidator<UpdateJobLevelCommand>
{
    public UpdateJobLevelCommandValidator()
    {
        RuleFor(x => x.LevelName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.LevelOrder).GreaterThan(0);
        RuleFor(x => x.Description).MaximumLength(500).When(x => x.Description is not null);
    }
}
