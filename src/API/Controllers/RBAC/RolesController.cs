namespace API;

[Authorize]
[ApiController]
[Route("api/roles")]
public sealed class RolesController(ISender sender) : ControllerBase
{
    [HasPermission(RolePermissions.ViewList)]
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<RoleResponse>>>> GetAll(CancellationToken ct)
        => Ok(ApiResponse<IEnumerable<RoleResponse>>.Ok(await sender.Send(new GetRolesQuery(), ct)));

    [HasPermission(RolePermissions.Create)]
    [HttpPost]
    public async Task<ActionResult<ApiResponse<Guid>>> Create(
        [FromBody] CreateRoleCommand cmd, CancellationToken ct)
        => Ok(ApiResponse<Guid>.Ok(await sender.Send(cmd, ct)));

    [HasPermission(RolePermissions.Update)]
    [HttpPut("{roleId:guid}")]
    public async Task<ActionResult<ApiResponse<Unit>>> Update(
        Guid roleId, [FromBody] UpdateRoleCommand cmd, CancellationToken ct)
        => Ok(ApiResponse<Unit>.Ok(await sender.Send(cmd with { RoleId = roleId }, ct)));

    [HasPermission(RolePermissions.Delete)]
    [HttpDelete("{roleId:guid}")]
    public async Task<ActionResult<ApiResponse<Unit>>> Delete(
        Guid roleId, CancellationToken ct)
        => Ok(ApiResponse<Unit>.Ok(await sender.Send(new DeleteRoleCommand(roleId), ct)));

    [HasPermission(RolePermissions.AssignPermission)]
    [HttpPut("{roleId:guid}/permissions")]
    public async Task<ActionResult<ApiResponse<Unit>>> AssignPermissions(
        Guid roleId, [FromBody] AssignPermissionsCommand cmd, CancellationToken ct)
        => Ok(ApiResponse<Unit>.Ok(await sender.Send(cmd with { RoleId = roleId }, ct)));

    [HasPermission(RolePermissions.ViewUsers)]
    [HttpGet("{roleId:guid}/users")]
    public async Task<ActionResult<ApiResponse<IEnumerable<UserSummaryResponse>>>> GetUsers(
        Guid roleId, CancellationToken ct)
        => Ok(ApiResponse<IEnumerable<UserSummaryResponse>>.Ok(await sender.Send(new GetRoleUsersQuery(roleId), ct)));

    [HasPermission(UserPermissions.AssignRole)]
    [HttpPut("{roleId:guid}/users")]
    public async Task<ActionResult<ApiResponse<Unit>>> SyncUsers(
        Guid roleId, [FromBody] BulkSyncRoleUsersCommand cmd, CancellationToken ct)
        => Ok(ApiResponse<Unit>.Ok(await sender.Send(cmd with { RoleId = roleId }, ct)));
}
