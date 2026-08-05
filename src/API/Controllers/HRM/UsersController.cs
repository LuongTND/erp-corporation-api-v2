namespace API;

[Authorize]
[ApiController]
[Route("api/users")]
public sealed class UsersController(ISender sender) : ControllerBase
{
    [HasPermission("users:create")]
    [HttpPost]
    public async Task<ActionResult<ApiResponse<Guid>>> CreateEmployee(
        [FromBody] CreateEmployeeCommand cmd, CancellationToken ct)
        => Ok(ApiResponse<Guid>.Ok(await sender.Send(cmd, ct)));

    [HasPermission("users:view")]
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<UserSummaryResponse>>>> GetUsers(
        [FromQuery] string? search, CancellationToken ct)
        => Ok(ApiResponse<IEnumerable<UserSummaryResponse>>.Ok(await sender.Send(new GetUsersQuery(search), ct)));

    // --- Department ---

    [HasPermission("users:assign-department")]
    [HttpPost("{userId:guid}/departments")]
    public async Task<ActionResult<ApiResponse<Guid>>> AddDepartment(
        Guid userId, [FromBody] AddUserDepartmentCommand cmd, CancellationToken ct)
        => Ok(ApiResponse<Guid>.Ok(await sender.Send(cmd with { UserId = userId }, ct)));

    [HasPermission("users:transfer-department")]
    [HttpPut("{userId:guid}/departments/transfer")]
    public async Task<ActionResult<ApiResponse<Unit>>> TransferDepartment(
        Guid userId, [FromBody] TransferUserDepartmentCommand cmd, CancellationToken ct)
        => Ok(ApiResponse<Unit>.Ok(await sender.Send(cmd with { UserId = userId }, ct)));

    [HasPermission("users:assign-department")]
    [HttpPut("{userId:guid}/departments/{departmentId:guid}")]
    public async Task<ActionResult<ApiResponse<Unit>>> UpdateDepartmentMembership(
        Guid userId, Guid departmentId, [FromBody] UpdateUserDepartmentCommand cmd, CancellationToken ct)
        => Ok(ApiResponse<Unit>.Ok(await sender.Send(cmd with { UserId = userId, DepartmentId = departmentId }, ct)));

    [HasPermission("users:assign-department")]
    [HttpDelete("{userId:guid}/departments/{departmentId:guid}")]
    public async Task<ActionResult<ApiResponse<Unit>>> RemoveDepartment(
        Guid userId, Guid departmentId, CancellationToken ct)
        => Ok(ApiResponse<Unit>.Ok(await sender.Send(new RemoveUserDepartmentCommand(userId, departmentId), ct)));

    // --- Role ---

    [HasPermission("users:assign-role")]
    [HttpPost("{userId:guid}/roles")]
    public async Task<ActionResult<ApiResponse<Guid>>> AssignRole(
        Guid userId, [FromBody] AssignRoleCommand cmd, CancellationToken ct)
        => Ok(ApiResponse<Guid>.Ok(await sender.Send(cmd with { UserId = userId }, ct)));

    [HasPermission("users:assign-role")]
    [HttpDelete("{userId:guid}/roles/{roleId:guid}")]
    public async Task<ActionResult<ApiResponse<Unit>>> RevokeRole(
        Guid userId, Guid roleId, CancellationToken ct)
        => Ok(ApiResponse<Unit>.Ok(await sender.Send(new RevokeRoleCommand(userId, roleId), ct)));

    // --- Scope Override ---

    [HasPermission("users:set-scope")]
    [HttpPut("{userId:guid}/scope")]
    public async Task<ActionResult<ApiResponse<Unit>>> SetScope(
        Guid userId, [FromBody] SetScopeOverrideCommand cmd, CancellationToken ct)
        => Ok(ApiResponse<Unit>.Ok(await sender.Send(cmd with { UserId = userId }, ct)));
}
