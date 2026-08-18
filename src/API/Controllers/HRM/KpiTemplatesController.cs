namespace API;

[Authorize]
[ApiController]
[Route("api/hrm/kpi-templates")]
public sealed class KpiTemplatesController(ISender sender) : ControllerBase
{
    [HasPermission(KpiTemplatePermissions.ViewList)]
    [HttpGet]
    public async Task<ActionResult<ApiResponse<QueryResult<KpiTemplateResponse>>>> GetList(
        [FromQuery] QueryInfo queryInfo,
        [FromQuery] Guid? departmentId,
        [FromQuery] Guid? jobLevelId,
        CancellationToken ct)
        => Ok(ApiResponse<QueryResult<KpiTemplateResponse>>.Ok(
            await sender.Send(new GetKpiTemplatesQuery(queryInfo, departmentId, jobLevelId), ct)));

    [HasPermission(KpiTemplatePermissions.ViewDetail)]
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<KpiTemplateResponse>>> GetById(Guid id, CancellationToken ct)
        => Ok(ApiResponse<KpiTemplateResponse>.Ok(await sender.Send(new GetKpiTemplateByIdQuery(id), ct)));

    [HasPermission(KpiTemplatePermissions.Create)]
    [HttpPost]
    public async Task<ActionResult<ApiResponse<Guid>>> Create(
        [FromBody] CreateKpiTemplateCommand cmd, CancellationToken ct)
        => Ok(ApiResponse<Guid>.Ok(await sender.Send(cmd, ct)));

    [HasPermission(KpiTemplatePermissions.Update)]
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResponse<string>>> Update(
        Guid id, [FromBody] UpdateKpiTemplateCommand cmd, CancellationToken ct)
    {
        await sender.Send(cmd with { Id = id }, ct);
        return Ok(ApiResponse<string>.Ok(BusinessMessages.UpdatedSuccessfully("KpiTemplate")));
    }

    [HasPermission(KpiTemplatePermissions.Delete)]
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ApiResponse<string>>> Delete(Guid id, CancellationToken ct)
    {
        await sender.Send(new DeleteKpiTemplateCommand(id), ct);
        return Ok(ApiResponse<string>.Ok(BusinessMessages.DeletedSuccessfully("KpiTemplate")));
    }
}
