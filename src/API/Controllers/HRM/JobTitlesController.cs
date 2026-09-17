namespace API;

[Authorize]
[ApiController]
[Route("api/job-titles")]
public sealed class JobLevelsController(ISender sender) : ControllerBase
{
    [HasPermission(JobTitlePermissions.ViewList)]
    [HttpGet]
    public async Task<ActionResult<ApiResponse<QueryResult<JobTitleResponse>>>> GetList(
        [FromQuery] QueryInfo queryInfo, CancellationToken ct)
        => Ok(ApiResponse<QueryResult<JobTitleResponse>>.Ok(
            await sender.Send(new GetJobTitlesQuery(queryInfo), ct)));

    [HasPermission(JobTitlePermissions.ViewDetail)]
    [HttpGet("{JobTitleId:guid}")]
    public async Task<ActionResult<ApiResponse<JobTitleResponse>>> GetById(
        Guid JobTitleId, CancellationToken ct)
        => Ok(ApiResponse<JobTitleResponse>.Ok(
            await sender.Send(new GetJobTitleByIdQuery(JobTitleId), ct)));

    [HasPermission(JobTitlePermissions.Create)]
    [HttpPost]
    public async Task<ActionResult<ApiResponse<Guid>>> Create(
        [FromBody] CreateJobTitleCommand cmd, CancellationToken ct)
        => Ok(ApiResponse<Guid>.Ok(await sender.Send(cmd, ct)));

    [HasPermission(JobTitlePermissions.Update)]
    [HttpPut("{JobTitleId:guid}")]
    public async Task<ActionResult<ApiResponse<Unit>>> Update(
        Guid JobTitleId, [FromBody] UpdateJobTitleCommand cmd, CancellationToken ct)
        => Ok(ApiResponse<Unit>.Ok(await sender.Send(cmd with { JobTitleId = JobTitleId }, ct)));

    [HasPermission(JobTitlePermissions.Delete)]
    [HttpDelete("{JobTitleId:guid}")]
    public async Task<ActionResult<ApiResponse<Unit>>> Delete(
        Guid JobTitleId, CancellationToken ct)
        => Ok(ApiResponse<Unit>.Ok(await sender.Send(new DeleteJobTitleCommand(JobTitleId), ct)));
}
