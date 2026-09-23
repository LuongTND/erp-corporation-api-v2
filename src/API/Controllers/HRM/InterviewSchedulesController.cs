namespace API;

[Authorize]
[ApiController]
[Route("api/interview-schedules")]
public sealed class InterviewSchedulesController(ISender sender) : ControllerBase
{
    [HasPermission(RecruitmentPermissions.ManageInterviewSchedule)]
    [HttpPost]
    public async Task<ActionResult<ApiResponse<Guid>>> Create(
        [FromBody] CreateInterviewScheduleCommand cmd, CancellationToken ct)
        => Ok(ApiResponse<Guid>.Ok(await sender.Send(cmd, ct)));

    [HasPermission(RecruitmentPermissions.ManageInterviewSchedule)]
    [HttpPut("{scheduleId:guid}")]
    public async Task<ActionResult<ApiResponse<Unit>>> Update(
        Guid scheduleId, [FromBody] UpdateInterviewScheduleCommand cmd, CancellationToken ct)
        => Ok(ApiResponse<Unit>.Ok(
            await sender.Send(cmd with { ScheduleId = scheduleId }, ct)));
}
