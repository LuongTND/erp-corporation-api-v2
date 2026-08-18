namespace Application;

public sealed class GetPermissionAuditLogsQueryHandler(IPermissionAuditLogRepository repo)
    : IRequestHandler<GetPermissionAuditLogsQuery, QueryResult<PermissionAuditLogResponse>>
{
    public Task<QueryResult<PermissionAuditLogResponse>> Handle(GetPermissionAuditLogsQuery query, CancellationToken ct)
        => repo.GetLogsAsync(query.Filter, ct);
}
