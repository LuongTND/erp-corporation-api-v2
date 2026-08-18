namespace API;

[Authorize]
[ApiController]
[Route("api/store-manager")]
public sealed class StoreManagerController(ISender sender) : ControllerBase
{
    [HasPermission(StoreManagerPermissions.ViewMyStore)]
    [HttpGet("my-store")]
    public async Task<ActionResult<ApiResponse<StorePortalResponse?>>> GetMyStore(CancellationToken ct)
        => Ok(ApiResponse<StorePortalResponse?>.Ok(await sender.Send(new GetMyStoreQuery(), ct)));

    [HasPermission(StoreManagerPermissions.ViewMyStoreMembers)]
    [HttpGet("my-store/members")]
    public async Task<ActionResult<ApiResponse<IEnumerable<StoreMemberResponse>>>> GetMyStoreMembers(CancellationToken ct)
        => Ok(ApiResponse<IEnumerable<StoreMemberResponse>>.Ok(
            await sender.Send(new GetMyStoreMembersQuery(), ct)));
}
