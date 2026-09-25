namespace Application;

public sealed class AddUserDepartmentCommandValidator : AbstractValidator<AddUserDepartmentCommand>
{
    public AddUserDepartmentCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.DepartmentId).NotEmpty();
        RuleFor(x => x.StartDate).NotEmpty();
    }
}
