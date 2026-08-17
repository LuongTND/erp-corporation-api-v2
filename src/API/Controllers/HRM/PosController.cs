namespace API;

[Authorize]
[ApiController]
[Route("api/pos")]
public sealed class PosController(ISender sender) : ControllerBase
{
    [HasPermission(StorePermissions.ImportFromPos)]
    [HttpGet("stores")]
    public async Task<ActionResult<ApiResponse<IEnumerable<PosStoreResponse>>>> GetPosStores(CancellationToken ct)
        => Ok(ApiResponse<IEnumerable<PosStoreResponse>>.Ok(await sender.Send(new GetPosStoresQuery(), ct)));

    [HasPermission(StorePermissions.ImportFromPos)]
    [HttpPost("stores/{posStoreId:guid}/import")]
    public async Task<ActionResult<ApiResponse<Guid>>> ImportPosStore(
        Guid posStoreId, [FromBody] ImportPosStoreCommand cmd, CancellationToken ct)
        => Ok(ApiResponse<Guid>.Ok(await sender.Send(cmd with { PosStoreId = posStoreId }, ct)));
}
