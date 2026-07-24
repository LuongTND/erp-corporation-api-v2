namespace Application;

public static class UnitOfWorkExtensions
{
    /// <summary>
    /// Save changes and throw <see cref="DatabaseOperationException"/> if no rows were affected.
    /// </summary>
    public static async Task EnsureSaveAsync(this IUnitOfWork uow, CancellationToken ct = default)
    {
        var rows = await uow.SaveChangesAsync(ct);
        if (rows < 1)
            throw new DatabaseOperationException(ExceptionMessages.DatabaseOperationFailed);
    }
}
