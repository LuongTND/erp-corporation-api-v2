namespace API;

[Authorize]
[ApiController]
[Route("api/stores")]
public sealed class StoresController(ISender sender) : ControllerBase
{
    [HasPermission("stores:view")]
    [HttpGet]
    public async Task<ActionResult<ApiResponse<QueryResult<StoreResponse>>>> GetStores(
        [FromQuery] QueryInfo query, [FromQuery] Guid? regionId, CancellationToken ct)
        => Ok(ApiResponse<QueryResult<StoreResponse>>.Ok(
            await sender.Send(new GetStoresQuery(query, regionId), ct)));

    [HasPermission("stores:delete")]
    [HttpDelete("{storeId:guid}")]
    public async Task<ActionResult<ApiResponse<Unit>>> DeleteStore(Guid storeId, CancellationToken ct)
        => Ok(ApiResponse<Unit>.Ok(await sender.Send(new DeleteStoreCommand(storeId), ct)));

    [HasPermission("stores:update")]
    [HttpPost("sync")]
    public async Task<ActionResult<ApiResponse<int>>> SyncPosStores(CancellationToken ct)
        => Ok(ApiResponse<int>.Ok(await sender.Send(new SyncPosStoresCommand(), ct)));

    [HasPermission("stores:view")]
    [HttpGet("{storeId:guid}/store-hours")]
    public async Task<ActionResult<ApiResponse<IEnumerable<StoreHoursResponse>>>> GetStoreHours(
        Guid storeId, CancellationToken ct)
        => Ok(ApiResponse<IEnumerable<StoreHoursResponse>>.Ok(
            await sender.Send(new GetStoreHoursQuery(storeId), ct)));

    [HasPermission("stores:update")]
    [HttpPatch("{storeId:guid}/toggle-active")]
    public async Task<ActionResult<ApiResponse<bool>>> ToggleStoreActive(Guid storeId, CancellationToken ct)
        => Ok(ApiResponse<bool>.Ok(await sender.Send(new ToggleStoreActiveCommand(storeId), ct)));

    [HasPermission("stores:update")]
    [HttpPut("{storeId:guid}/store-hours")]
    public async Task<ActionResult<ApiResponse<Unit>>> UpsertStoreHours(
        Guid storeId, [FromBody] UpsertStoreHoursCommand cmd, CancellationToken ct)
        => Ok(ApiResponse<Unit>.Ok(await sender.Send(cmd with { StoreId = storeId }, ct)));

    [HasPermission("stores:update")]
    [HttpPatch("{storeId:guid}/manager")]
    public async Task<ActionResult<ApiResponse<Unit>>> AssignManager(
        Guid storeId, [FromBody] AssignStoreManagerCommand cmd, CancellationToken ct)
        => Ok(ApiResponse<Unit>.Ok(await sender.Send(cmd with { StoreId = storeId }, ct)));

    [HasPermission("stores:view")]
    [HttpGet("{storeId:guid}/members")]
    public async Task<ActionResult<ApiResponse<IEnumerable<StoreMemberResponse>>>> GetMembers(
        Guid storeId, CancellationToken ct)
        => Ok(ApiResponse<IEnumerable<StoreMemberResponse>>.Ok(
            await sender.Send(new GetStoreMembersQuery(storeId), ct)));

    [HasPermission("stores:update")]
    [HttpPost("{storeId:guid}/members")]
    public async Task<ActionResult<ApiResponse<Guid>>> AddMember(
        Guid storeId, [FromBody] AddStoreMemberCommand cmd, CancellationToken ct)
        => Ok(ApiResponse<Guid>.Ok(await sender.Send(cmd with { StoreId = storeId }, ct)));

    [HasPermission("stores:update")]
    [HttpDelete("{storeId:guid}/members/{userId:guid}")]
    public async Task<ActionResult<ApiResponse<Unit>>> RemoveMember(
        Guid storeId, Guid userId, CancellationToken ct)
        => Ok(ApiResponse<Unit>.Ok(await sender.Send(new RemoveStoreMemberCommand(storeId, userId), ct)));
}
