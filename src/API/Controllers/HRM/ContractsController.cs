namespace API;

[Authorize]
[ApiController]
[Route("api/hrm/users/{userId:guid}/contracts")]
public sealed class ContractsController(ISender sender) : ControllerBase
{
    [HasPermission(ContractPermissions.View)]
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<EmploymentContractResponse>>>> GetAll(
        Guid userId, CancellationToken ct)
        => Ok(ApiResponse<IEnumerable<EmploymentContractResponse>>.Ok(
            await sender.Send(new GetContractsQuery(userId), ct)));

    [HasPermission(ContractPermissions.Create)]
    [HttpPost]
    public async Task<ActionResult<ApiResponse<Guid>>> Create(
        Guid userId,
        [FromForm] ContractType type,
        [FromForm] DateOnly startDate,
        [FromForm] DateOnly? endDate,
        [FromForm] decimal salary,
        [FromForm] decimal? salaryForSocialInsurance,
        [FromForm] string? positionTitle,
        [FromForm] DateOnly? signedDate,
        [FromForm] Guid? templateId,
        IFormFile file,
        CancellationToken ct)
    {
        var cmd = new CreateContractCommand(
            userId, type, startDate, endDate, salary,
            salaryForSocialInsurance, positionTitle, signedDate,
            templateId, file.OpenReadStream(), file.FileName, file.ContentType);

        return Ok(ApiResponse<Guid>.Ok(await sender.Send(cmd, ct)));
    }

    [HasPermission(ContractPermissions.Renew)]
    [HttpPost("{contractId:guid}/renew")]
    public async Task<ActionResult<ApiResponse<Guid>>> Renew(
        Guid userId, Guid contractId,
        [FromForm] ContractType type,
        [FromForm] DateOnly startDate,
        [FromForm] DateOnly? endDate,
        [FromForm] decimal salary,
        [FromForm] decimal? salaryForSocialInsurance,
        [FromForm] string? positionTitle,
        [FromForm] DateOnly? signedDate,
        IFormFile file,
        CancellationToken ct)
    {
        var cmd = new RenewContractCommand(
            userId, contractId, type, startDate, endDate, salary,
            salaryForSocialInsurance, positionTitle, signedDate,
            file.OpenReadStream(), file.FileName, file.ContentType);
        return Ok(ApiResponse<Guid>.Ok(await sender.Send(cmd, ct)));
    }

    [HasPermission(ContractPermissions.Terminate)]
    [HttpPost("{contractId:guid}/terminate")]
    public async Task<ActionResult<ApiResponse<object?>>> Terminate(
        Guid userId, Guid contractId, [FromBody] TerminateContractCommand cmd, CancellationToken ct)
    {
        await sender.Send(cmd with { UserId = userId, ContractId = contractId }, ct);
        return Ok(ApiResponse<object?>.Ok(null));
    }

    [HasPermission(ContractPermissions.View)]
    [HttpGet("salary-comparison")]
    public async Task<ActionResult<ApiResponse<ContractSalaryComparisonResponse>>> GetSalaryComparison(
        Guid userId, CancellationToken ct)
        => Ok(ApiResponse<ContractSalaryComparisonResponse>.Ok(
            await sender.Send(new GetSalaryComparisonQuery(userId), ct)));
}

[Authorize]
[ApiController]
[Route("api/hrm/contracts")]
public sealed class ContractManagementController(ISender sender) : ControllerBase
{
    [HasPermission(ContractPermissions.View)]
    [HttpGet("expiring")]
    public async Task<ActionResult<ApiResponse<IEnumerable<EmploymentContractResponse>>>> GetExpiring(
        [FromQuery] int days = 30, CancellationToken ct = default)
        => Ok(ApiResponse<IEnumerable<EmploymentContractResponse>>.Ok(
            await sender.Send(new GetExpiringContractsQuery(days), ct)));
}
