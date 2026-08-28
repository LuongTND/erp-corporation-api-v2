namespace API;

[Authorize]
[ApiController]
[Route("api/candidates/{candidateId:guid}/interviews")]
public sealed class InterviewSchedulesController(ISender sender) : ControllerBase
{
    [HasPermission(RecruitmentPermissions.ManageInterviewSchedule)]
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<InterviewScheduleResponse>>>> GetList(
        Guid candidateId, CancellationToken ct)
        => Ok(ApiResponse<IEnumerable<InterviewScheduleResponse>>.Ok(
            await sender.Send(new GetInterviewSchedulesQuery(candidateId), ct)));

    [HasPermission(RecruitmentPermissions.ManageInterviewSchedule)]
    [HttpPost]
    public async Task<ActionResult<ApiResponse<Guid>>> Create(
        Guid candidateId, [FromBody] CreateInterviewScheduleCommand cmd, CancellationToken ct)
        => Ok(ApiResponse<Guid>.Ok(await sender.Send(cmd with { CandidateId = candidateId }, ct)));

    [HasPermission(RecruitmentPermissions.CompleteInterviewSchedule)]
    [HttpPost("{scheduleId:guid}/complete")]
    public async Task<ActionResult<ApiResponse<Unit>>> Complete(
        Guid candidateId, Guid scheduleId, [FromBody] CompleteInterviewScheduleCommand cmd, CancellationToken ct)
        => Ok(ApiResponse<Unit>.Ok(await sender.Send(cmd with { ScheduleId = scheduleId }, ct)));

    [HasPermission(RecruitmentPermissions.ManageInterviewSchedule)]
    [HttpPost("{scheduleId:guid}/cancel")]
    public async Task<ActionResult<ApiResponse<Unit>>> Cancel(
        Guid candidateId, Guid scheduleId, [FromBody] CancelInterviewScheduleCommand cmd, CancellationToken ct)
        => Ok(ApiResponse<Unit>.Ok(await sender.Send(cmd with { ScheduleId = scheduleId }, ct)));
}
