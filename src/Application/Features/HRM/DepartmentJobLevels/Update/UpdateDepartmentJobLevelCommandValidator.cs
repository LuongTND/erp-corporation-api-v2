namespace Application;

public sealed class UpdateDepartmentJobLevelCommandValidator : AbstractValidator<UpdateDepartmentJobLevelCommand>
{
    public UpdateDepartmentJobLevelCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}
