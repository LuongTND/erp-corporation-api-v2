namespace API;

[Authorize]
[ApiController]
[Route("api/hrm/department-job-levels")]
public sealed class DepartmentJobLevelsController(ISender sender) : ControllerBase
{
    [HasPermission(DepartmentJobLevelPermissions.ViewList)]
    [HttpGet]
    public async Task<ActionResult<ApiResponse<QueryResult<DepartmentJobLevelResponse>>>> GetList(
        [FromQuery] QueryInfo queryInfo,
        [FromQuery] Guid? departmentId,
        CancellationToken ct)
        => Ok(ApiResponse<QueryResult<DepartmentJobLevelResponse>>.Ok(
            await sender.Send(new GetDepartmentJobLevelsQuery(queryInfo, departmentId), ct)));

    [HasPermission(DepartmentJobLevelPermissions.ViewDetail)]
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<DepartmentJobLevelResponse>>> GetById(Guid id, CancellationToken ct)
        => Ok(ApiResponse<DepartmentJobLevelResponse>.Ok(
            await sender.Send(new GetDepartmentJobLevelByIdQuery(id), ct)));

    [HasPermission(DepartmentJobLevelPermissions.Create)]
    [HttpPost]
    public async Task<ActionResult<ApiResponse<Guid>>> Create(
        [FromBody] CreateDepartmentJobLevelCommand cmd, CancellationToken ct)
        => Ok(ApiResponse<Guid>.Ok(await sender.Send(cmd, ct)));

    [HasPermission(DepartmentJobLevelPermissions.Update)]
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResponse<string>>> Update(
        Guid id, [FromBody] UpdateDepartmentJobLevelCommand cmd, CancellationToken ct)
    {
        await sender.Send(cmd with { Id = id }, ct);
        return Ok(ApiResponse<string>.Ok(BusinessMessages.UpdatedSuccessfully("DepartmentJobLevel")));
    }

    [HasPermission(DepartmentJobLevelPermissions.Delete)]
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ApiResponse<string>>> Delete(Guid id, CancellationToken ct)
    {
        await sender.Send(new DeleteDepartmentJobLevelCommand(id), ct);
        return Ok(ApiResponse<string>.Ok(BusinessMessages.DeletedSuccessfully("DepartmentJobLevel")));
    }

    [HasPermission(DepartmentJobLevelPermissions.AssignKpiTemplate)]
    [HttpPost("{id:guid}/assign-kpi-template")]
    public async Task<ActionResult<ApiResponse<string>>> AssignKpiTemplate(
        Guid id, [FromBody] AssignKpiTemplateCommand cmd, CancellationToken ct)
    {
        await sender.Send(cmd with { DepartmentJobLevelId = id }, ct);
        return Ok(ApiResponse<string>.Ok(BusinessMessages.UpdatedSuccessfully("KpiTemplate assignment")));
    }
}
