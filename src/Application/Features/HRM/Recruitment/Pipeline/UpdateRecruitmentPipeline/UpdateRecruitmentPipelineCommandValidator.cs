namespace Application;

public sealed class UpdateRecruitmentPipelineCommandValidator
    : AbstractValidator<UpdateRecruitmentPipelineCommand>
{
    public UpdateRecruitmentPipelineCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
    }
}
