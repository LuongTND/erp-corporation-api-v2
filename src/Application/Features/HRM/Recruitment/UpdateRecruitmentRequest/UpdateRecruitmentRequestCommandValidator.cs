namespace Application;

public sealed class UpdateRecruitmentRequestCommandValidator
    : AbstractValidator<UpdateRecruitmentRequestCommand>
{
    public UpdateRecruitmentRequestCommandValidator()
    {
        RuleFor(x => x.PositionTitle).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Headcount).GreaterThan(0);
        RuleFor(x => x.Reason).NotEmpty().MaximumLength(2000);
    }
}
