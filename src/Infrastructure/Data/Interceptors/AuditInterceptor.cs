using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Infrastructure;

public class AuditInterceptor(IHttpContextAccessor httpContextAccessor) : SaveChangesInterceptor
{
    private static readonly HashSet<string> _skipProperties =
    [
        nameof(IEntity.CreatedAt), nameof(IEntity.ModifiedAt),
        nameof(ISoftDeletable.DeletedAt), nameof(ISoftDeletable.IsDeleted),
        nameof(IAuditable.CreatedBy), nameof(IAuditable.UpdatedBy)
    ];

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context is not null)
            WriteAuditLogs(eventData.Context);

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        if (eventData.Context is not null)
            WriteAuditLogs(eventData.Context);

        return base.SavingChanges(eventData, result);
    }

    private void WriteAuditLogs(DbContext context)
    {
        var userId = GetCurrentUserId();
        var now = DateTimeOffset.UtcNow;

        var entries = context.ChangeTracker.Entries<IAuditable>()
            .Where(e => e.State is EntityState.Added or EntityState.Modified or EntityState.Deleted)
            .ToList();

        var logs = new List<AuditLog>();

        foreach (var entry in entries)
        {
            var tableName = entry.Metadata.GetTableName() ?? entry.Metadata.ClrType.Name;
            var entityId = entry.Properties
                .FirstOrDefault(p => p.Metadata.IsPrimaryKey())?.CurrentValue?.ToString() ?? "unknown";

            if (entry.State is EntityState.Added)
            {
                logs.Add(new AuditLog
                {
                    TableName = tableName,
                    EntityId = entityId,
                    Action = "Created",
                    UserId = userId,
                    Timestamp = now
                });
                continue;
            }

            if (entry.State is EntityState.Deleted)
            {
                logs.Add(new AuditLog
                {
                    TableName = tableName,
                    EntityId = entityId,
                    Action = "Deleted",
                    UserId = userId,
                    Timestamp = now
                });
                continue;
            }

            // Modified — capture field-level changes
            foreach (var prop in entry.Properties)
            {
                if (_skipProperties.Contains(prop.Metadata.Name)) continue;
                if (prop.Metadata.IsKey()) continue;
                if (!prop.IsModified) continue;

                var oldVal = prop.OriginalValue?.ToString();
                var newVal = prop.CurrentValue?.ToString();
                if (oldVal == newVal) continue;

                logs.Add(new AuditLog
                {
                    TableName = tableName,
                    EntityId = entityId,
                    Action = "Updated",
                    FieldName = prop.Metadata.Name,
                    OldValue = oldVal,
                    NewValue = newVal,
                    UserId = userId,
                    Timestamp = now
                });
            }
        }

        if (logs.Count > 0)
            context.Set<AuditLog>().AddRange(logs);
    }

    private Guid? GetCurrentUserId()
    {
        var value = httpContextAccessor.HttpContext?.User.FindFirst("user_id")?.Value;
        return Guid.TryParse(value, out var id) ? id : null;
    }
}
