namespace API;

[Authorize]
[ApiController]
[Route("api/audit-logs")]
public sealed class AuditLogsController(ISender sender) : ControllerBase
{
    [HasPermission("roles:view")]
    [HttpGet]
    public async Task<ActionResult<ApiResponse<QueryResult<PermissionAuditLogResponse>>>> GetAll(
        [FromQuery] string? action,
        [FromQuery] Guid? actorId,
        [FromQuery] Guid? roleId,
        [FromQuery] DateTimeOffset? from,
        [FromQuery] DateTimeOffset? to,
        [FromQuery] int top = 15,
        [FromQuery] int skip = 0,
        CancellationToken ct = default)
        => Ok(ApiResponse<QueryResult<PermissionAuditLogResponse>>.Ok(await sender.Send(
            new GetPermissionAuditLogsQuery(new PermissionAuditLogFilter
            {
                Action = action,
                ActorId = actorId,
                RoleId = roleId,
                From = from,
                To = to,
                Top = top,
                Skip = skip,
            }), ct)));
}
