namespace API;

[Authorize]
[ApiController]
[Route("api/permissions")]
public sealed class PermissionsController(ISender sender) : ControllerBase
{
    [HasPermission("roles:view")]
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<PermissionResponse>>>> GetAll(CancellationToken ct)
        => Ok(ApiResponse<IEnumerable<PermissionResponse>>.Ok(await sender.Send(new GetPermissionsQuery(), ct)));

    [HasPermission("roles:delete")]
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ApiResponse<Unit>>> Delete(Guid id, CancellationToken ct)
    {
        await sender.Send(new DeletePermissionCommand(id), ct);
        return Ok(ApiResponse<Unit>.Ok(Unit.Value));
    }
}
