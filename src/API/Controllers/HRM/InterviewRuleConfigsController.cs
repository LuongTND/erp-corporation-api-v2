namespace API;

[Authorize]
[ApiController]
[Route("api/interview-rule-configs")]
public sealed class InterviewRuleConfigsController(ISender sender) : ControllerBase
{
    [HasPermission(RecruitmentPermissions.ManageInterviewRule)]
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<InterviewRuleConfigResponse>>>> GetList(
        [FromQuery] RecruitmentRequestContext? context,
        [FromQuery] bool? isActive,
        CancellationToken ct)
        => Ok(ApiResponse<IEnumerable<InterviewRuleConfigResponse>>.Ok(
            await sender.Send(new GetInterviewRuleConfigsQuery(context, isActive), ct)));

    [HasPermission(RecruitmentPermissions.ManageInterviewRule)]
    [HttpPost]
    public async Task<ActionResult<ApiResponse<Guid>>> Create(
        [FromBody] CreateInterviewRuleConfigCommand cmd, CancellationToken ct)
        => Ok(ApiResponse<Guid>.Ok(await sender.Send(cmd, ct)));

    [HasPermission(RecruitmentPermissions.ManageInterviewRule)]
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResponse<Unit>>> Update(
        Guid id, [FromBody] UpdateInterviewRuleConfigCommand cmd, CancellationToken ct)
        => Ok(ApiResponse<Unit>.Ok(await sender.Send(cmd with { Id = id }, ct)));

    // Resolve rule tự động cho 1 candidate
    [HasPermission(RecruitmentPermissions.ManageInterviewSchedule)]
    [HttpGet("resolve")]
    public async Task<ActionResult<ApiResponse<InterviewRuleConfigResponse?>>> Resolve(
        [FromQuery] Guid candidateId, CancellationToken ct)
        => Ok(ApiResponse<InterviewRuleConfigResponse?>.Ok(
            await sender.Send(new ResolveInterviewRuleQuery(candidateId), ct)));
}
