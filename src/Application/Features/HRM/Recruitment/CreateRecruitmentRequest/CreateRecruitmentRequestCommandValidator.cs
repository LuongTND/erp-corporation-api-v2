namespace Application;

public sealed class CreateRecruitmentRequestCommandValidator
    : AbstractValidator<CreateRecruitmentRequestCommand>
{
    public CreateRecruitmentRequestCommandValidator()
    {
        RuleFor(x => x.PositionTitle).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Headcount).GreaterThan(0);
        RuleFor(x => x.Reason).NotEmpty().MaximumLength(2000);
        RuleFor(x => x.JobDescription).MaximumLength(5000).When(x => x.JobDescription is not null);

        RuleFor(x => x.DepartmentId)
            .NotNull().WithMessage("DepartmentId bắt buộc khi RequestContext = Department.")
            .When(x => x.RequestContext == RecruitmentRequestContext.Department);

        RuleFor(x => x.StoreId)
            .NotNull().WithMessage("StoreId bắt buộc khi RequestContext = Store.")
            .When(x => x.RequestContext == RecruitmentRequestContext.Store);
    }
}
