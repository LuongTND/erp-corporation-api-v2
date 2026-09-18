namespace Application;

public sealed class UpdateJobTitleCommandValidator : AbstractValidator<UpdateJobTitleCommand>
{
    public UpdateJobTitleCommandValidator()
    {
        RuleFor(x => x.JobTitleId)
            .NotEmpty().WithMessage("ID chức danh không được để trống.");

        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Mã chức danh không được để trống.")
            .MinimumLength(2).WithMessage("Mã chức danh phải có ít nhất 2 ký tự.")
            .MaximumLength(50).WithMessage("Mã chức danh không được vượt quá 50 ký tự.")
            .Matches(@"^[A-Z0-9\-]+$").WithMessage("Mã chức danh chỉ được chứa chữ hoa, số và dấu gạch ngang.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Tên chức danh không được để trống.")
            .MinimumLength(2).WithMessage("Tên chức danh phải có ít nhất 2 ký tự.")
            .MaximumLength(100).WithMessage("Tên chức danh không được vượt quá 100 ký tự.");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Mô tả không được vượt quá 500 ký tự.")
            .When(x => x.Description is not null);

        RuleFor(x => x.Level)
            .IsInEnum().WithMessage("Cấp chức danh không hợp lệ.");

        RuleFor(x => x.UnitType)
            .IsInEnum().WithMessage("Loại đơn vị áp dụng không hợp lệ.");
    }
}
