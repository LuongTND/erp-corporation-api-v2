namespace API;

[Authorize]
[ApiController]
[Route("api/hrm/kpi-entries")]
public sealed class KpiEntriesController(ISender sender) : ControllerBase
{
    [HasPermission("kpi-entries:view")]
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<KpiEntryResponse>>>> GetList(
        [FromQuery] int month,
        [FromQuery] int year,
        [FromQuery] Guid? userId,
        [FromQuery] Guid? kpiMetricId,
        CancellationToken ct)
        => Ok(ApiResponse<IReadOnlyList<KpiEntryResponse>>.Ok(
            await sender.Send(new GetKpiEntriesQuery(month, year, userId, kpiMetricId), ct)));

    [HasPermission("kpi-entries:view")]
    [HttpGet("summary")]
    public async Task<ActionResult<ApiResponse<KpiSummaryResponse>>> GetSummary(
        [FromQuery] Guid userId,
        [FromQuery] int month,
        [FromQuery] int year,
        CancellationToken ct)
        => Ok(ApiResponse<KpiSummaryResponse>.Ok(
            await sender.Send(new GetKpiSummaryQuery(userId, month, year), ct)));

    [HasPermission("kpi-entries:create")]
    [HttpPost]
    public async Task<ActionResult<ApiResponse<Guid>>> Upsert(
        [FromBody] UpsertKpiEntryCommand cmd, CancellationToken ct)
        => Ok(ApiResponse<Guid>.Ok(await sender.Send(cmd, ct)));
}
