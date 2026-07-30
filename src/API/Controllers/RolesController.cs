namespace API;

[Authorize]
[ApiController]
[Route("api/roles")]
public sealed class RolesController(ISender sender) : ControllerBase
{
    [HasPermission("roles:view")]
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<RoleResponse>>>> GetAll(CancellationToken ct)
        => Ok(ApiResponse<IEnumerable<RoleResponse>>.Ok(await sender.Send(new GetRolesQuery(), ct)));

    [HasPermission("roles:create")]
    [HttpPost]
    public async Task<ActionResult<ApiResponse<Guid>>> Create(
        [FromBody] CreateRoleCommand cmd, CancellationToken ct)
        => Ok(ApiResponse<Guid>.Ok(await sender.Send(cmd, ct)));

    [HasPermission("roles:update")]
    [HttpPut("{roleId:guid}")]
    public async Task<ActionResult<ApiResponse<Unit>>> Update(
        Guid roleId, [FromBody] UpdateRoleCommand cmd, CancellationToken ct)
        => Ok(ApiResponse<Unit>.Ok(await sender.Send(cmd with { RoleId = roleId }, ct)));

    [HasPermission("roles:delete")]
    [HttpDelete("{roleId:guid}")]
    public async Task<ActionResult<ApiResponse<Unit>>> Delete(
        Guid roleId, CancellationToken ct)
        => Ok(ApiResponse<Unit>.Ok(await sender.Send(new DeleteRoleCommand(roleId), ct)));

    [HasPermission("roles:assign-permission")]
    [HttpPut("{roleId:guid}/permissions")]
    public async Task<ActionResult<ApiResponse<Unit>>> AssignPermissions(
        Guid roleId, [FromBody] AssignPermissionsCommand cmd, CancellationToken ct)
        => Ok(ApiResponse<Unit>.Ok(await sender.Send(cmd with { RoleId = roleId }, ct)));
}
