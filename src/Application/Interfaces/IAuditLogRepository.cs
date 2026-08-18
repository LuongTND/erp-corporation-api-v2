namespace Application;

public interface IAuditLogRepository
{
    Task<QueryResult<AuditLogResponse>> GetAuditLogsAsync(AuditLogFilter filter, CancellationToken ct = default);
}
