namespace API;

[Authorize]
[ApiController]
[Route("api/store-manager")]
public sealed class StoreManagerController(ISender sender) : ControllerBase
{
    [HttpGet("my-store")]
    public async Task<ActionResult<ApiResponse<StorePortalResponse?>>> GetMyStore(CancellationToken ct)
        => Ok(ApiResponse<StorePortalResponse?>.Ok(await sender.Send(new GetMyStoreQuery(), ct)));
}
