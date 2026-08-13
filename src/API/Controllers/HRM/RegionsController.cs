namespace API;

[Authorize]
[ApiController]
[Route("api/regions")]
public sealed class RegionsController(ISender sender) : ControllerBase
{
    [HasPermission("regions:view")]
    [HttpGet]
    public async Task<ActionResult<ApiResponse<QueryResult<RegionResponse>>>> GetRegions(
        [FromQuery] QueryInfo query, CancellationToken ct)
        => Ok(ApiResponse<QueryResult<RegionResponse>>.Ok(
            await sender.Send(new GetRegionsQuery(query), ct)));

    [HasPermission("regions:update")]
    [HttpPost("sync")]
    public async Task<ActionResult<ApiResponse<int>>> SyncPosRegions(CancellationToken ct)
        => Ok(ApiResponse<int>.Ok(await sender.Send(new SyncPosRegionsCommand(), ct)));

    [HasPermission("regions:view")]
    [HttpGet("{regionId:guid}/region-hours")]
    public async Task<ActionResult<ApiResponse<IEnumerable<RegionHoursResponse>>>> GetRegionHours(
        Guid regionId, CancellationToken ct)
        => Ok(ApiResponse<IEnumerable<RegionHoursResponse>>.Ok(
            await sender.Send(new GetRegionHoursQuery(regionId), ct)));

    [HasPermission("regions:update")]
    [HttpPut("{regionId:guid}/region-hours")]
    public async Task<ActionResult<ApiResponse<Unit>>> UpsertRegionHours(
        Guid regionId, [FromBody] UpsertRegionHoursCommand cmd, CancellationToken ct)
        => Ok(ApiResponse<Unit>.Ok(await sender.Send(cmd with { RegionId = regionId }, ct)));
}
