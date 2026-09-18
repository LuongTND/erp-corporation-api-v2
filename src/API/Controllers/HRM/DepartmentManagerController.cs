namespace API;

[Authorize]
[ApiController]
[Route("api/department-manager")]
public sealed class DepartmentManagerController(ISender sender) : ControllerBase
{
    /// <summary>Lấy danh sách phòng ban mà user hiện tại đang là trưởng phòng.</summary>
    [HasPermission(DepartmentManagerPermissions.ViewMyDepartments)]
    [HttpGet("my-departments")]
    public async Task<ActionResult<ApiResponse<IEnumerable<DepartmentResponse>>>> GetMyDepartments(CancellationToken ct)
        => Ok(ApiResponse<IEnumerable<DepartmentResponse>>.Ok(
            await sender.Send(new GetMyDepartmentsQuery(), ct)));
}
