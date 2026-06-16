using Application.DTOs.Permissions;
using FluentValidation;

namespace Application.Validators.Permissions;

public class CreatePermissionRequestValidator : AbstractValidator<CreatePermissionRequest>
{
    public CreatePermissionRequestValidator()
    {
        RuleFor(x => x.PermissionCode)
            .NotEmpty().WithMessage("Mã quyền không được để trống.")
            .MaximumLength(100).WithMessage("Mã quyền không được vượt quá 100 ký tự.")
            .Matches(@"^[a-z0-9._]+$").WithMessage("Mã quyền chỉ gồm chữ thường, số, dấu chấm và gạch dưới.");

        RuleFor(x => x.PermissionName)
            .NotEmpty().WithMessage("Tên quyền không được để trống.")
            .MaximumLength(200).WithMessage("Tên quyền không được vượt quá 200 ký tự.");

        RuleFor(x => x.Module).IsInEnum().WithMessage("Module không hợp lệ.");
        RuleFor(x => x.Action).IsInEnum().WithMessage("Hành động không hợp lệ.");

        RuleFor(x => x.Resource)
            .NotEmpty().WithMessage("Resource không được để trống.")
            .MaximumLength(50).WithMessage("Resource không được vượt quá 50 ký tự.");
    }
}

public class UpdatePermissionRequestValidator : AbstractValidator<UpdatePermissionRequest>
{
    public UpdatePermissionRequestValidator()
    {
        RuleFor(x => x.PermissionName)
            .NotEmpty().WithMessage("Tên quyền không được để trống.")
            .MaximumLength(200).WithMessage("Tên quyền không được vượt quá 200 ký tự.");
    }
}
