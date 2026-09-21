namespace Application;

public sealed class CreateRecruitmentPipelineCommandValidator
    : AbstractValidator<CreateRecruitmentPipelineCommand>
{
    public CreateRecruitmentPipelineCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
    }
}
