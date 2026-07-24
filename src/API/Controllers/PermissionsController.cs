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
}
