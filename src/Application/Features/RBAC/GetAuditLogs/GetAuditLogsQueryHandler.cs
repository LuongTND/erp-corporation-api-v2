namespace Application;

public sealed class GetAuditLogsQueryHandler(IAuditLogRepository auditLogRepository)
    : IRequestHandler<GetAuditLogsQuery, QueryResult<AuditLogResponse>>
{
    public Task<QueryResult<AuditLogResponse>> Handle(GetAuditLogsQuery query, CancellationToken ct)
        => auditLogRepository.GetAuditLogsAsync(query.Filter, ct);
}
