namespace Application;

public class UpdateRoleRequestValidator : AbstractValidator<UpdateRoleRequest>
{
    public UpdateRoleRequestValidator()
    {
        RuleFor(x => x.DisplayName)
            .NotEmpty().WithMessage("Tên hiển thị vai trò không được để trống.")
            .MaximumLength(200).WithMessage("Tên hiển thị không được vượt quá 200 ký tự.");
    }
}