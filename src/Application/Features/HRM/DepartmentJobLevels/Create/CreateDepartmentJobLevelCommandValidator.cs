namespace Application;

public sealed class CreateDepartmentJobLevelCommandValidator : AbstractValidator<CreateDepartmentJobLevelCommand>
{
    public CreateDepartmentJobLevelCommandValidator()
    {
        RuleFor(x => x.DepartmentId).NotEmpty();
        RuleFor(x => x.JobLevelId).NotEmpty();
    }
}
