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
        [FromQuery] string? search, [FromQuery] Guid? jobLevelId, [FromQuery] UserStatus? status, [FromQuery] Guid? departmentId, CancellationToken ct)
        => Ok(ApiResponse<IEnumerable<UserSummaryResponse>>.Ok(
            await sender.Send(new GetUsersQuery(search, jobLevelId, status, departmentId), ct)));

    [HasPermission("users:view")]
    [HttpGet("{userId:guid}")]
    public async Task<ActionResult<ApiResponse<UserDetailResponse>>> GetUserDetail(
        Guid userId, CancellationToken ct)
        => Ok(ApiResponse<UserDetailResponse>.Ok(await sender.Send(new GetUserDetailQuery(userId), ct)));

    [HasPermission("users:edit")]
    [HttpPut("{userId:guid}")]
    public async Task<ActionResult<ApiResponse<Unit>>> UpdateEmployee(
        Guid userId, [FromBody] UpdateEmployeeCommand cmd, CancellationToken ct)
        => Ok(ApiResponse<Unit>.Ok(await sender.Send(cmd with { UserId = userId }, ct)));

    [HasPermission("users:edit")]
    [HttpPost("{userId:guid}/avatar")]
    [RequestSizeLimit(5 * 1024 * 1024)] // 5MB
    public async Task<ActionResult<ApiResponse<string>>> UploadAvatar(
        Guid userId, [FromForm] IFormFile file, CancellationToken ct)
    {
        using var stream = file.OpenReadStream();
        var url = await sender.Send(new UploadAvatarCommand(userId, stream, file.ContentType, file.FileName), ct);
        return Ok(ApiResponse<string>.Ok(url));
    }

    [HasPermission("users:edit")]
    [HttpPatch("{userId:guid}/custom-fields")]
    public async Task<ActionResult<ApiResponse<Unit>>> UpsertCustomFields(
        Guid userId, [FromBody] IEnumerable<CustomFieldValueInput> values, CancellationToken ct)
        => Ok(ApiResponse<Unit>.Ok(await sender.Send(new UpsertUserCustomFieldValuesCommand(userId, values), ct)));

    [HasPermission("users:edit")]
    [HttpDelete("{userId:guid}/job-level")]
    public async Task<ActionResult<ApiResponse<Unit>>> UnassignJobLevel(
        Guid userId, CancellationToken ct)
        => Ok(ApiResponse<Unit>.Ok(await sender.Send(new UnassignJobLevelCommand(userId), ct)));

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

    // --- Status ---

    [HasPermission("users:edit")]
    [HttpPatch("{userId:guid}/status")]
    public async Task<ActionResult<ApiResponse<Unit>>> UpdateStatus(
        Guid userId, [FromBody] UpdateUserStatusCommand cmd, CancellationToken ct)
        => Ok(ApiResponse<Unit>.Ok(await sender.Send(cmd with { UserId = userId }, ct)));

    [HasPermission("users:view")]
    [HttpGet("{userId:guid}/status-history")]
    public async Task<ActionResult<ApiResponse<IEnumerable<UserStatusHistoryResponse>>>> GetStatusHistory(
        Guid userId, CancellationToken ct)
        => Ok(ApiResponse<IEnumerable<UserStatusHistoryResponse>>.Ok(
            await sender.Send(new GetUserStatusHistoryQuery(userId), ct)));

    // --- Work History ---

    [HasPermission("users:view")]
    [HttpGet("{userId:guid}/work-history")]
    public async Task<ActionResult<ApiResponse<IEnumerable<WorkHistoryResponse>>>> GetWorkHistory(
        Guid userId, [FromQuery] WorkHistoryChangeType? changeType, CancellationToken ct)
        => Ok(ApiResponse<IEnumerable<WorkHistoryResponse>>.Ok(
            await sender.Send(new GetWorkHistoryQuery(userId, changeType), ct)));

    // --- Lock ---

    [HasPermission("users:edit")]
    [HttpPatch("{userId:guid}/lock")]
    public async Task<ActionResult<ApiResponse<Unit>>> LockEmployee(
        Guid userId, [FromBody] LockEmployeeCommand cmd, CancellationToken ct)
        => Ok(ApiResponse<Unit>.Ok(await sender.Send(cmd with { UserId = userId }, ct)));

    // --- Export ---

    [HasPermission("users:view")]
    [HttpGet("export")]
    public async Task<IActionResult> ExportUsers(
        [FromQuery] string? search, [FromQuery] UserStatus? status, [FromQuery] Guid? departmentId, CancellationToken ct)
    {
        var bytes = await sender.Send(new ExportUsersQuery(search, status, departmentId), ct);
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"nhan-su-{DateTimeOffset.UtcNow:yyyyMMdd}.xlsx");
    }
}