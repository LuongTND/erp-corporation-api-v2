namespace API;

[Authorize]
[ApiController]
[Route("api/employee-types")]
public sealed class EmployeeTypesController(ISender sender) : ControllerBase
{
    [HasPermission(EmployeeTypePermissions.ViewList)]
    [HttpGet]
    public async Task<ActionResult<ApiResponse<QueryResult<EmployeeTypeResponse>>>> GetList(
        [FromQuery] QueryInfo queryInfo, CancellationToken ct)
        => Ok(ApiResponse<QueryResult<EmployeeTypeResponse>>.Ok(
            await sender.Send(new GetEmployeeTypesQuery(queryInfo), ct)));

    [HasPermission(EmployeeTypePermissions.Create)]
    [HttpPost]
    public async Task<ActionResult<ApiResponse<Guid>>> Create(
        [FromBody] CreateEmployeeTypeCommand cmd, CancellationToken ct)
        => Ok(ApiResponse<Guid>.Ok(await sender.Send(cmd, ct)));

    [HasPermission(EmployeeTypePermissions.Update)]
    [HttpPut("{employeeTypeId:guid}")]
    public async Task<ActionResult<ApiResponse<Unit>>> Update(
        Guid employeeTypeId, [FromBody] UpdateEmployeeTypeCommand cmd, CancellationToken ct)
        => Ok(ApiResponse<Unit>.Ok(await sender.Send(cmd with { EmployeeTypeId = employeeTypeId }, ct)));

    [HasPermission(EmployeeTypePermissions.Delete)]
    [HttpDelete("{employeeTypeId:guid}")]
    public async Task<ActionResult<ApiResponse<Unit>>> Delete(
        Guid employeeTypeId, CancellationToken ct)
        => Ok(ApiResponse<Unit>.Ok(await sender.Send(new DeleteEmployeeTypeCommand(employeeTypeId), ct)));
}
