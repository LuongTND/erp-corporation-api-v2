namespace Application;

public sealed class ImportPosStoreCommandValidator : AbstractValidator<ImportPosStoreCommand>
{
    public ImportPosStoreCommandValidator()
    {
        RuleFor(x => x.PosStoreId).NotEmpty();
    }
}
