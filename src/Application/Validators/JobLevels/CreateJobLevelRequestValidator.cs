namespace Application;

public class CreateJobLevelRequestValidator : AbstractValidator<CreateJobLevelRequest>
{
    public CreateJobLevelRequestValidator()
    {
        RuleFor(x => x.LevelName)
            .NotEmpty().WithMessage("Tên cấp bậc không được để trống.")
            .MaximumLength(100).WithMessage("Tên cấp bậc không được vượt quá 100 ký tự.");

        RuleFor(x => x.LevelOrder)
            .GreaterThan(0).WithMessage("Thứ tự cấp bậc phải lớn hơn 0.");

        RuleFor(x => x.DefaultScopeType)
            .IsInEnum().WithMessage("Phạm vi dữ liệu mặc định không hợp lệ.");
    }
}