namespace API;

[Authorize]
[ApiController]
[Route("api/job-postings")]
public sealed class JobPostingsController(ISender sender) : ControllerBase
{
    [HasPermission(RecruitmentPermissions.ManageJobPosting)]
    [HttpGet]
    public async Task<ActionResult<ApiResponse<QueryResult<JobPostingResponse>>>> GetList(
        [FromQuery] QueryInfo queryInfo,
        [FromQuery] JobPostingCostStatus? costStatus,
        [FromQuery] Guid? recruitmentRequestId,
        CancellationToken ct)
        => Ok(ApiResponse<QueryResult<JobPostingResponse>>.Ok(
            await sender.Send(new GetAllJobPostingsQuery(queryInfo, costStatus, recruitmentRequestId), ct)));
}
