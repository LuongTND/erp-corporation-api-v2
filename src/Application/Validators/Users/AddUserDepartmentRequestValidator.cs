using Application.DTOs.Users;
using FluentValidation;

namespace Application.Validators.Users;

public class AddUserDepartmentRequestValidator : AbstractValidator<AddUserDepartmentRequest>
{
    public AddUserDepartmentRequestValidator()
    {
        RuleFor(x => x.DepartmentId)
            .NotEmpty().WithMessage("Phòng ban không được để trống.");

        RuleFor(x => x.StartDate)
            .NotEmpty().WithMessage("Ngày bắt đầu kiêm nhiệm không được để trống.");

        RuleFor(x => (AddUserDepartmentRequest)x)
            .Must(x => !x.EndDate.HasValue || x.EndDate.Value >= x.StartDate)
            .WithMessage("Ngày kết thúc kiêm nhiệm phải lớn hơn hoặc bằng ngày bắt đầu.");
    }
}
