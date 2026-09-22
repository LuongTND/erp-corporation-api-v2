namespace API;

[Authorize]
[ApiController]
[Route("api/job-postings")]
public sealed class JobPostingsController(ISender sender) : ControllerBase
{
    [HasPermission(RecruitmentPermissions.ManageJobPosting)]
    [HttpPost]
    public async Task<ActionResult<ApiResponse<Guid>>> Create(
        [FromBody] CreateJobPostingCommand cmd, CancellationToken ct)
        => Ok(ApiResponse<Guid>.Ok(await sender.Send(cmd, ct)));

    [HasPermission(RecruitmentPermissions.ViewRequest)]
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<JobPostingResponse>>>> GetList(
        [FromQuery] Guid recruitmentRequestId, CancellationToken ct)
        => Ok(ApiResponse<IEnumerable<JobPostingResponse>>.Ok(
            await sender.Send(new GetJobPostingsQuery(recruitmentRequestId), ct)));

    [HasPermission(RecruitmentPermissions.ViewRequest)]
    [HttpGet("{postingId:guid}")]
    public async Task<ActionResult<ApiResponse<JobPostingResponse>>> GetById(
        Guid postingId, CancellationToken ct)
        => Ok(ApiResponse<JobPostingResponse>.Ok(
            await sender.Send(new GetJobPostingDetailQuery(postingId), ct)));

    [HasPermission(RecruitmentPermissions.ManageJobPosting)]
    [HttpPut("{postingId:guid}")]
    public async Task<ActionResult<ApiResponse<Unit>>> Update(
        Guid postingId, [FromBody] UpdateJobPostingCommand cmd, CancellationToken ct)
        => Ok(ApiResponse<Unit>.Ok(await sender.Send(cmd with { PostingId = postingId }, ct)));

    [HasPermission(RecruitmentPermissions.ManageJobPosting)]
    [HttpDelete("{postingId:guid}")]
    public async Task<ActionResult<ApiResponse<Unit>>> Delete(
        Guid postingId, CancellationToken ct)
        => Ok(ApiResponse<Unit>.Ok(await sender.Send(new DeleteJobPostingCommand(postingId), ct)));
}
