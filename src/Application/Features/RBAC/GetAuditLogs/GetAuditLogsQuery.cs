namespace Application;

public sealed record GetAuditLogsQuery(AuditLogFilter Filter) : IRequest<QueryResult<AuditLogResponse>>;
