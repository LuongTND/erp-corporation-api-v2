namespace API;

[Authorize]
[ApiController]
[Route("api/hrm/bonus-policies")]
public sealed class BonusPoliciesController(ISender sender) : ControllerBase
{
    [HasPermission("bonus-policies:view")]
    [HttpGet]
    public async Task<ActionResult<ApiResponse<QueryResult<BonusPolicyResponse>>>> GetList(
        [FromQuery] QueryInfo queryInfo, CancellationToken ct)
        => Ok(ApiResponse<QueryResult<BonusPolicyResponse>>.Ok(
            await sender.Send(new GetBonusPoliciesQuery(queryInfo), ct)));

    [HasPermission("bonus-policies:create")]
    [HttpPost]
    public async Task<ActionResult<ApiResponse<Guid>>> Create(
        [FromBody] CreateBonusPolicyCommand cmd, CancellationToken ct)
        => Ok(ApiResponse<Guid>.Ok(await sender.Send(cmd, ct)));
}
