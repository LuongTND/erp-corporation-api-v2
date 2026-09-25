namespace Application;

public sealed class UpdateDepartmentCommandValidator : AbstractValidator<UpdateDepartmentCommand>
{
    public UpdateDepartmentCommandValidator()
    {
        RuleFor(x => x.DepartmentName).NotEmpty().MaximumLength(255);
        RuleFor(x => x.DepartmentCode).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Description).MaximumLength(1000).When(x => x.Description is not null);
    }
}
