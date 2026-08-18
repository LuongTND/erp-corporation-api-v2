namespace API;

[Authorize]
[ApiController]
[Route("api/departments")]
public sealed class DepartmentsController(ISender sender) : ControllerBase
{
    [HasPermission(DepartmentPermissions.ViewList)]
    [HttpGet]
    public async Task<ActionResult<ApiResponse<QueryResult<DepartmentResponse>>>> GetList(
        [FromQuery] QueryInfo queryInfo, CancellationToken ct)
        => Ok(ApiResponse<QueryResult<DepartmentResponse>>.Ok(
            await sender.Send(new GetDepartmentsQuery(queryInfo), ct)));

    [HasPermission(DepartmentPermissions.ViewDetail)]
    [HttpGet("{departmentId:guid}")]
    public async Task<ActionResult<ApiResponse<DepartmentResponse>>> GetById(
        Guid departmentId, CancellationToken ct)
        => Ok(ApiResponse<DepartmentResponse>.Ok(
            await sender.Send(new GetDepartmentByIdQuery(departmentId), ct)));

    [HasPermission(DepartmentPermissions.ViewTree)]
    [HttpGet("tree")]
    public async Task<ActionResult<ApiResponse<IEnumerable<DepartmentTreeResponse>>>> GetTree(
        CancellationToken ct)
        => Ok(ApiResponse<IEnumerable<DepartmentTreeResponse>>.Ok(
            await sender.Send(new GetDepartmentTreeQuery(), ct)));

    [HasPermission(DepartmentPermissions.Create)]
    [HttpPost]
    public async Task<ActionResult<ApiResponse<Guid>>> Create(
        [FromBody] CreateDepartmentCommand cmd, CancellationToken ct)
        => Ok(ApiResponse<Guid>.Ok(await sender.Send(cmd, ct)));

    [HasPermission(DepartmentPermissions.Update)]
    [HttpPut("{departmentId:guid}")]
    public async Task<ActionResult<ApiResponse<Unit>>> Update(
        Guid departmentId, [FromBody] UpdateDepartmentCommand cmd, CancellationToken ct)
        => Ok(ApiResponse<Unit>.Ok(await sender.Send(cmd with { DepartmentId = departmentId }, ct)));

    [HasPermission(DepartmentPermissions.Delete)]
    [HttpDelete("{departmentId:guid}")]
    public async Task<ActionResult<ApiResponse<Unit>>> Delete(
        Guid departmentId, CancellationToken ct)
        => Ok(ApiResponse<Unit>.Ok(await sender.Send(new DeleteDepartmentCommand(departmentId), ct)));

    [HasPermission(DepartmentPermissions.ViewMembers)]
    [HttpGet("{departmentId:guid}/members")]
    public async Task<ActionResult<ApiResponse<IEnumerable<DepartmentMemberResponse>>>> GetMembers(
        Guid departmentId, CancellationToken ct)
        => Ok(ApiResponse<IEnumerable<DepartmentMemberResponse>>.Ok(
            await sender.Send(new GetDepartmentMembersQuery(departmentId), ct)));

    [HasPermission(UserPermissions.AddDepartment)]
    [HttpPost("{departmentId:guid}/members/bulk")]
    public async Task<ActionResult<ApiResponse<int>>> AddMembersBulk(
        Guid departmentId, [FromBody] AddBulkUserDepartmentCommand cmd, CancellationToken ct)
        => Ok(ApiResponse<int>.Ok(await sender.Send(cmd with { DepartmentId = departmentId }, ct)));
}
