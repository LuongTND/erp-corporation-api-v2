namespace Application;

public static class UnitOfWorkExtensions
{
    public static async Task EnsureSaveAsync(this IUnitOfWork uow, CancellationToken ct = default)
    {
        var rows = await uow.SaveChangesAsync(ct);
        if (rows < 1)
            throw new InvalidOperationException("Database operation failed: no rows were affected.");
    }
}
