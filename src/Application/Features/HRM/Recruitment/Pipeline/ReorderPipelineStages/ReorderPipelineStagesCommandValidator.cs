namespace Application;

public sealed class ReorderPipelineStagesCommandValidator
    : AbstractValidator<ReorderPipelineStagesCommand>
{
    public ReorderPipelineStagesCommandValidator()
    {
        RuleFor(x => x.PipelineId).NotEmpty();
        RuleFor(x => x.Items).NotNull().NotEmpty();
        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(i => i.StageId).NotEmpty();
            item.RuleFor(i => i.DisplayOrder).GreaterThan(0);
        });
    }
}
