namespace Application;

public sealed class UpdatePipelineStageCommandValidator : AbstractValidator<UpdatePipelineStageCommand>
{
    public UpdatePipelineStageCommandValidator()
    {
        RuleFor(x => x.StageId).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
    }
}
