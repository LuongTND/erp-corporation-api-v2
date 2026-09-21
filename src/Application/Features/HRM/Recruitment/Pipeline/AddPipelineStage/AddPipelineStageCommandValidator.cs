namespace Application;

public sealed class AddPipelineStageCommandValidator : AbstractValidator<AddPipelineStageCommand>
{
    public AddPipelineStageCommandValidator()
    {
        RuleFor(x => x.PipelineId).NotEmpty();
        RuleFor(x => x.RoundTypeId).NotEmpty();
        RuleFor(x => x.Name).MaximumLength(200).When(x => x.Name is not null);
        RuleFor(x => x.DisplayOrder).GreaterThan(0);
    }
}
