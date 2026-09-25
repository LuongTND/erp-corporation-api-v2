namespace Application;

public sealed class TransferUserDepartmentCommandValidator : AbstractValidator<TransferUserDepartmentCommand>
{
    public TransferUserDepartmentCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.NewDepartmentId).NotEmpty();
        RuleFor(x => x.TransferDate).NotEmpty();
    }
}
