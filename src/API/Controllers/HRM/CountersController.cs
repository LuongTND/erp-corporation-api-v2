namespace API;

[Authorize]
[ApiController]
[Route("api/counters")]
public sealed class CountersController(ISender sender) : ControllerBase
{
    [HasPermission(CounterPermissions.ViewList)]
    [HttpGet]
    public async Task<ActionResult<ApiResponse<QueryResult<CounterResponse>>>> GetCounters(
        [FromQuery] QueryInfo query, [FromQuery] Guid? storeId, CancellationToken ct)
        => Ok(ApiResponse<QueryResult<CounterResponse>>.Ok(
            await sender.Send(new GetCountersQuery(storeId, query), ct)));

    [HasPermission(CounterPermissions.Create)]
    [HttpPost]
    public async Task<ActionResult<ApiResponse<Guid>>> CreateCounter(
        [FromBody] CreateCounterCommand cmd, CancellationToken ct)
        => Ok(ApiResponse<Guid>.Ok(await sender.Send(cmd, ct)));

    [HasPermission(CounterPermissions.Update)]
    [HttpPut("{counterId:guid}")]
    public async Task<ActionResult<ApiResponse<Unit>>> UpdateCounter(
        Guid counterId, [FromBody] UpdateCounterCommand cmd, CancellationToken ct)
        => Ok(ApiResponse<Unit>.Ok(await sender.Send(cmd with { CounterId = counterId }, ct)));

    [HasPermission(CounterPermissions.Update)]
    [HttpPatch("{counterId:guid}/toggle-active")]
    public async Task<ActionResult<ApiResponse<bool>>> ToggleCounterActive(Guid counterId, CancellationToken ct)
        => Ok(ApiResponse<bool>.Ok(await sender.Send(new ToggleCounterActiveCommand(counterId), ct)));

    [HasPermission(CounterPermissions.Delete)]
    [HttpDelete("{counterId:guid}")]
    public async Task<ActionResult<ApiResponse<Unit>>> DeleteCounter(Guid counterId, CancellationToken ct)
        => Ok(ApiResponse<Unit>.Ok(await sender.Send(new DeleteCounterCommand(counterId), ct)));
}
