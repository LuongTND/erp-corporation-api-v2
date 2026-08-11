namespace API;

[Authorize]
[ApiController]
[Route("api/hrm/payroll-runs")]
public sealed class PayrollRunsController(ISender sender) : ControllerBase
{
    [HasPermission("payroll-runs:view")]
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<PayrollRunResponse>>>> GetList(
        [FromQuery] int? year,
        CancellationToken ct)
        => Ok(ApiResponse<IReadOnlyList<PayrollRunResponse>>.Ok(
            await sender.Send(new GetPayrollRunsQuery(year), ct)));

    [HasPermission("payroll-runs:view")]
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<PayrollRunDetailResponse>>> GetById(Guid id, CancellationToken ct)
        => Ok(ApiResponse<PayrollRunDetailResponse>.Ok(
            await sender.Send(new GetPayrollRunByIdQuery(id), ct)));

    [HasPermission("payroll-runs:create")]
    [HttpPost]
    public async Task<ActionResult<ApiResponse<Guid>>> Create(
        [FromBody] CreatePayrollRunCommand cmd, CancellationToken ct)
        => Ok(ApiResponse<Guid>.Ok(await sender.Send(cmd, ct)));

    [HasPermission("payroll-runs:update")]
    [HttpPut("entries/{entryId:guid}")]
    public async Task<ActionResult<ApiResponse<string>>> UpdateEntry(
        Guid entryId, [FromBody] UpdatePayrollEntryCommand cmd, CancellationToken ct)
    {
        await sender.Send(cmd with { EntryId = entryId }, ct);
        return Ok(ApiResponse<string>.Ok(BusinessMessages.UpdatedSuccessfully("PayrollEntry")));
    }

    [HasPermission("payroll-runs:finalize")]
    [HttpPost("{id:guid}/finalize")]
    public async Task<ActionResult<ApiResponse<string>>> Finalize(Guid id, CancellationToken ct)
    {
        await sender.Send(new FinalizePayrollRunCommand(id), ct);
        return Ok(ApiResponse<string>.Ok("Bảng lương đã được chốt."));
    }
}
