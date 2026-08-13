namespace Infrastructure;

[RegisterService(typeof(IPermissionAuditLogRepository))]
public sealed class PermissionAuditLogRepository(ApplicationDbContext db) : IPermissionAuditLogRepository
{
    public async Task WriteAsync(PermissionAuditLog log, CancellationToken ct = default)
    {
        db.PermissionAuditLogs.Add(log);
        await db.SaveChangesAsync(ct);
    }

    public async Task<QueryResult<PermissionAuditLogResponse>> GetLogsAsync(PermissionAuditLogFilter filter, CancellationToken ct = default)
    {
        var query = db.PermissionAuditLogs.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(filter.Action))
            query = query.Where(x => x.Action == filter.Action);
        if (filter.ActorId.HasValue)
            query = query.Where(x => x.ActorId == filter.ActorId.Value);
        if (filter.RoleId.HasValue)
            query = query.Where(x => x.RoleId == filter.RoleId.Value);
        if (filter.From.HasValue)
            query = query.Where(x => x.OccurredAt >= filter.From.Value);
        if (filter.To.HasValue)
            query = query.Where(x => x.OccurredAt <= filter.To.Value);

        var totalCount = await query.CountAsync(ct);

        var items = await query
            .OrderByDescending(x => x.OccurredAt)
            .Skip(filter.Skip)
            .Take(filter.Top)
            .Select(x => new PermissionAuditLogResponse
            {
                Id = x.Id,
                Action = x.Action,
                ActorId = x.ActorId,
                ActorName = x.ActorName,
                TargetUserId = x.TargetUserId,
                TargetUserName = x.TargetUserName,
                RoleId = x.RoleId,
                RoleName = x.RoleName,
                PermissionCodes = x.PermissionCodes,
                Detail = x.Detail,
                OccurredAt = x.OccurredAt,
            })
            .ToListAsync(ct);

        return new QueryResult<PermissionAuditLogResponse> { Items = items, TotalCount = totalCount };
    }
}
