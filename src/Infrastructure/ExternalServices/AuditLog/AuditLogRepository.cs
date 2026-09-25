namespace Infrastructure;

[RegisterService(typeof(IAuditLogRepository))]
public sealed class AuditLogRepository(ApplicationDbContext db) : IAuditLogRepository
{
    public async Task<QueryResult<AuditLogResponse>> GetAuditLogsAsync(AuditLogFilter filter, CancellationToken ct = default)
    {
        var query = db.AuditLogs.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(filter.TableName))
            query = query.Where(a => a.TableName == filter.TableName);

        if (!string.IsNullOrWhiteSpace(filter.Action))
            query = query.Where(a => a.Action == filter.Action);

        if (filter.UserId.HasValue)
            query = query.Where(a => a.UserId == filter.UserId);

        if (filter.From.HasValue)
            query = query.Where(a => a.Timestamp >= filter.From.Value);

        if (filter.To.HasValue)
            query = query.Where(a => a.Timestamp <= filter.To.Value);

        var totalCount = filter.NeedTotalCount ? await query.CountAsync(ct) : 0;

        var items = await query
            .OrderByDescending(a => a.Timestamp)
            .Skip(filter.Skip)
            .Take(filter.Top)
            .Select(a => new AuditLogResponse
            {
                Id = a.Id,
                TableName = a.TableName,
                EntityId = a.EntityId,
                Action = a.Action,
                FieldName = a.FieldName,
                OldValue = a.OldValue,
                NewValue = a.NewValue,
                UserId = a.UserId,
                Timestamp = a.Timestamp,
            })
            .ToListAsync(ct);

        return new QueryResult<AuditLogResponse>
        {
            Items = items,
            TotalCount = totalCount,
        };
    }
}
