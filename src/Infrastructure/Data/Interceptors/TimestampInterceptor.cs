namespace Infrastructure;

public class TimestampInterceptor : SaveChangesInterceptor
{
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context is not null)
            UpdateTimestamps(eventData.Context);

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        if (eventData.Context is not null)
            UpdateTimestamps(eventData.Context);

        return base.SavingChanges(eventData, result);
    }

    private static void UpdateTimestamps(DbContext context)
    {
        var now = DateTimeOffset.UtcNow;

        foreach (var entry in context.ChangeTracker.Entries())
        {
            if (entry.Entity is ISoftDeletable softDeletable && entry.State == EntityState.Deleted)
            {
                entry.State = EntityState.Modified;
                softDeletable.IsDeleted = true;
                softDeletable.DeletedAt = now;
                continue;
            }

            if (entry.Entity is not IEntity entity) continue;

            if (entry.State == EntityState.Added)
            {
                entity.CreatedAt = now;
                entity.ModifiedAt = null;
                if (entity is ISoftDeletable s) { s.IsDeleted = false; s.DeletedAt = null; }
            }
            else if (entry.State == EntityState.Modified)
            {
                entity.ModifiedAt = now;
                entry.Property(nameof(IEntity.CreatedAt)).IsModified = false;
            }
        }
    }

}