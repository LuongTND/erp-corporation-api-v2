namespace API;

[Authorize]
[ApiController]
[Route("api/hrm/users/{userId:guid}/salary")]
public sealed class SalaryController(ISender sender) : ControllerBase
{
    [HasPermission(SalaryPermissions.View)]
    [HttpGet("current")]
    public async Task<ActionResult<ApiResponse<SalaryRecordResponse?>>> GetCurrent(Guid userId, CancellationToken ct)
        => Ok(ApiResponse<SalaryRecordResponse?>.Ok(await sender.Send(new GetCurrentSalaryQuery(userId), ct)));

    [HasPermission(SalaryPermissions.View)]
    [HttpGet("history")]
    public async Task<ActionResult<ApiResponse<IEnumerable<SalaryRecordResponse>>>> GetHistory(Guid userId, CancellationToken ct)
        => Ok(ApiResponse<IEnumerable<SalaryRecordResponse>>.Ok(await sender.Send(new GetSalaryHistoryQuery(userId), ct)));

    [HasPermission(SalaryPermissions.Set)]
    [HttpPost]
    public async Task<ActionResult<ApiResponse<Guid>>> Set(
        Guid userId, [FromBody] SetSalaryCommand cmd, CancellationToken ct)
        => Ok(ApiResponse<Guid>.Ok(await sender.Send(cmd with { UserId = userId }, ct)));
}
