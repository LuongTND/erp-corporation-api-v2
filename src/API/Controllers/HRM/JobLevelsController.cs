namespace API;

[Authorize]
[ApiController]
[Route("api/job-levels")]
public sealed class JobLevelsController(ISender sender) : ControllerBase
{
    [HasPermission(JobLevelPermissions.ViewList)]
    [HttpGet]
    public async Task<ActionResult<ApiResponse<QueryResult<JobLevelResponse>>>> GetList(
        [FromQuery] QueryInfo queryInfo, CancellationToken ct)
        => Ok(ApiResponse<QueryResult<JobLevelResponse>>.Ok(
            await sender.Send(new GetJobLevelsQuery(queryInfo), ct)));

    [HasPermission(JobLevelPermissions.ViewDetail)]
    [HttpGet("{jobLevelId:guid}")]
    public async Task<ActionResult<ApiResponse<JobLevelResponse>>> GetById(
        Guid jobLevelId, CancellationToken ct)
        => Ok(ApiResponse<JobLevelResponse>.Ok(
            await sender.Send(new GetJobLevelByIdQuery(jobLevelId), ct)));

    [HasPermission(JobLevelPermissions.Create)]
    [HttpPost]
    public async Task<ActionResult<ApiResponse<Guid>>> Create(
        [FromBody] CreateJobLevelCommand cmd, CancellationToken ct)
        => Ok(ApiResponse<Guid>.Ok(await sender.Send(cmd, ct)));

    [HasPermission(JobLevelPermissions.Update)]
    [HttpPut("{jobLevelId:guid}")]
    public async Task<ActionResult<ApiResponse<Unit>>> Update(
        Guid jobLevelId, [FromBody] UpdateJobLevelCommand cmd, CancellationToken ct)
        => Ok(ApiResponse<Unit>.Ok(await sender.Send(cmd with { JobLevelId = jobLevelId }, ct)));

    [HasPermission(JobLevelPermissions.Delete)]
    [HttpDelete("{jobLevelId:guid}")]
    public async Task<ActionResult<ApiResponse<Unit>>> Delete(
        Guid jobLevelId, CancellationToken ct)
        => Ok(ApiResponse<Unit>.Ok(await sender.Send(new DeleteJobLevelCommand(jobLevelId), ct)));
}
