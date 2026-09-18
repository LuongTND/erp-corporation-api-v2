namespace Application;

public sealed class RequestMoreInfoRecruitmentCommandValidator
    : AbstractValidator<RequestMoreInfoRecruitmentCommand>
{
    public RequestMoreInfoRecruitmentCommandValidator()
    {
        RuleFor(x => x.NeedMoreInfoNote).NotEmpty().MaximumLength(2000);
    }
}
