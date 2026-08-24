namespace API;

[Authorize]
[ApiController]
[Route("api/permissions")]
public sealed class PermissionsController(ISender sender) : ControllerBase
{
    [HasPermission(PermissionPermissions.ViewList)]
    [HttpGet]
    public async Task<ActionResult<ApiResponse<QueryResult<PermissionResponse>>>> GetAll(
        [FromQuery] string? search,
        [FromQuery] int skip = 0,
        [FromQuery] int top = 0,
        CancellationToken ct = default)
        => Ok(ApiResponse<QueryResult<PermissionResponse>>.Ok(
            await sender.Send(new GetPermissionsQuery { SearchText = search, Skip = skip, Top = top }, ct)));

    [HasPermission(PermissionPermissions.Delete)]
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ApiResponse<Unit>>> Delete(Guid id, CancellationToken ct)
    {
        await sender.Send(new DeletePermissionCommand(id), ct);
        return Ok(ApiResponse<Unit>.Ok(Unit.Value));
    }
}
