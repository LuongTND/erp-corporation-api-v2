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

    [HasPermission("roles:assign-permission")]
    [HttpPut("{roleId:guid}/permissions")]
    public async Task<ActionResult<ApiResponse<MediatR.Unit>>> AssignPermissions(
        Guid roleId, [FromBody] AssignPermissionsCommand cmd, CancellationToken ct)
        => Ok(ApiResponse<MediatR.Unit>.Ok(await sender.Send(cmd with { RoleId = roleId }, ct)));
}
