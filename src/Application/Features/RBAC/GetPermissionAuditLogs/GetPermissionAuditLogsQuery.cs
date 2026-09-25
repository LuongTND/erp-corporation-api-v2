namespace Application;

public sealed record GetPermissionAuditLogsQuery(PermissionAuditLogFilter Filter)
    : IRequest<QueryResult<PermissionAuditLogResponse>>;
