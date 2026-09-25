namespace Application;

public interface IPermissionAuditLogRepository
{
    Task WriteAsync(PermissionAuditLog log, CancellationToken ct = default);
    Task<QueryResult<PermissionAuditLogResponse>> GetLogsAsync(PermissionAuditLogFilter filter, CancellationToken ct = default);
}
