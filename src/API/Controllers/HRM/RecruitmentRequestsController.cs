namespace API;

[Authorize]
[ApiController]
[Route("api/recruitment-requests")]
public sealed class RecruitmentRequestsController(ISender sender) : ControllerBase
{
    // HRM-045 / HRM-046 / HRM-047 / HRM-048 / HRM-049
    [HasPermission(RecruitmentPermissions.CreateRequest)]
    [HttpPost]
    public async Task<ActionResult<ApiResponse<Guid>>> Create(
        [FromBody] CreateRecruitmentRequestCommand cmd, CancellationToken ct)
        => Ok(ApiResponse<Guid>.Ok(await sender.Send(cmd, ct)));

    [HasPermission(RecruitmentPermissions.ViewRequest)]
    [HttpGet]
    public async Task<ActionResult<ApiResponse<QueryResult<RecruitmentRequestResponse>>>> GetList(
        [FromQuery] QueryInfo queryInfo,
        [FromQuery] RecruitmentRequestStatus? status,
        [FromQuery] RecruitmentRequestContext? requestContext,
        [FromQuery] Guid? departmentId,
        [FromQuery] Guid? storeId,
        [FromQuery] Guid? requestedByUserId,
        CancellationToken ct)
        => Ok(ApiResponse<QueryResult<RecruitmentRequestResponse>>.Ok(
            await sender.Send(new GetRecruitmentRequestsQuery(queryInfo, status, requestContext, departmentId, storeId, requestedByUserId), ct)));

    // HRM-054: kèm approval history
    [HasPermission(RecruitmentPermissions.ViewRequest)]
    [HttpGet("{requestId:guid}")]
    public async Task<ActionResult<ApiResponse<RecruitmentRequestDetailResponse>>> GetById(
        Guid requestId, CancellationToken ct)
        => Ok(ApiResponse<RecruitmentRequestDetailResponse>.Ok(
            await sender.Send(new GetRecruitmentRequestDetailQuery(requestId), ct)));

    [HasPermission(RecruitmentPermissions.UpdateRequest)]
    [HttpPut("{requestId:guid}")]
    public async Task<ActionResult<ApiResponse<Unit>>> Update(
        Guid requestId, [FromBody] UpdateRecruitmentRequestCommand cmd, CancellationToken ct)
        => Ok(ApiResponse<Unit>.Ok(await sender.Send(cmd with { RequestId = requestId }, ct)));

    // HRM-050
    [HasPermission(RecruitmentPermissions.SubmitRequest)]
    [HttpPost("{requestId:guid}/submit")]
    public async Task<ActionResult<ApiResponse<Unit>>> Submit(
        Guid requestId, CancellationToken ct)
        => Ok(ApiResponse<Unit>.Ok(await sender.Send(new SubmitRecruitmentRequestCommand(requestId), ct)));

    // HRM-051: Duyệt phiếu — workflow engine tự xác định bước hiện tại và guard AssignedTo
    [Authorize]
    [HttpPost("{requestId:guid}/approve")]
    public async Task<ActionResult<ApiResponse<Unit>>> Approve(
        Guid requestId, [FromBody] ApproveRecruitmentRequestCommand cmd, CancellationToken ct)
        => Ok(ApiResponse<Unit>.Ok(await sender.Send(cmd with { RequestId = requestId }, ct)));

    // HRM-052
    [HasPermission(RecruitmentPermissions.RejectRequest)]
    [HttpPost("{requestId:guid}/reject")]
    public async Task<ActionResult<ApiResponse<Unit>>> Reject(
        Guid requestId, [FromBody] RejectRecruitmentRequestCommand cmd, CancellationToken ct)
        => Ok(ApiResponse<Unit>.Ok(await sender.Send(cmd with { RequestId = requestId }, ct)));

    // HRM-053
    [HasPermission(RecruitmentPermissions.RequestMoreInfo)]
    [HttpPost("{requestId:guid}/request-more-info")]
    public async Task<ActionResult<ApiResponse<Unit>>> RequestMoreInfo(
        Guid requestId, [FromBody] RequestMoreInfoRecruitmentCommand cmd, CancellationToken ct)
        => Ok(ApiResponse<Unit>.Ok(await sender.Send(cmd with { RequestId = requestId }, ct)));

    // Người tạo huỷ phiếu đang chờ duyệt
    [HasPermission(RecruitmentPermissions.SubmitRequest)]
    [HttpPost("{requestId:guid}/cancel")]
    public async Task<ActionResult<ApiResponse<Unit>>> Cancel(
        Guid requestId, [FromBody] CancelRecruitmentRequestCommand cmd, CancellationToken ct)
        => Ok(ApiResponse<Unit>.Ok(await sender.Send(cmd with { RequestId = requestId }, ct)));

    [HasPermission(RecruitmentPermissions.CreateRequest)]
    [HttpDelete("{requestId:guid}")]
    public async Task<ActionResult<ApiResponse<Unit>>> Delete(
        Guid requestId, [FromQuery] string? note, CancellationToken ct)
        => Ok(ApiResponse<Unit>.Ok(await sender.Send(new DeleteRecruitmentRequestCommand(requestId, note), ct)));

    // HRM-055 / HRM-056 / HRM-057
    [HasPermission(RecruitmentPermissions.ManageJobPosting)]
    [HttpPost("{requestId:guid}/job-postings")]
    public async Task<ActionResult<ApiResponse<Guid>>> CreateJobPosting(
        Guid requestId, [FromBody] CreateJobPostingCommand cmd, CancellationToken ct)
        => Ok(ApiResponse<Guid>.Ok(await sender.Send(cmd with { RecruitmentRequestId = requestId }, ct)));

    [HasPermission(RecruitmentPermissions.ViewRequest)]
    [HttpGet("{requestId:guid}/job-postings")]
    public async Task<ActionResult<ApiResponse<QueryResult<JobPostingResponse>>>> GetJobPostings(
        Guid requestId, CancellationToken ct)
        => Ok(ApiResponse<QueryResult<JobPostingResponse>>.Ok(
            await sender.Send(new GetJobPostingsQuery(requestId), ct)));

    // HRM-058: duyệt chi phí
    [HasPermission(RecruitmentPermissions.ApprovePaidPosting)]
    [HttpPost("{requestId:guid}/job-postings/{postingId:guid}/approve-cost")]
    public async Task<ActionResult<ApiResponse<Unit>>> ApprovePostingCost(
        Guid requestId, Guid postingId, CancellationToken ct)
        => Ok(ApiResponse<Unit>.Ok(await sender.Send(
            new ApproveJobPostingCostCommand(postingId), ct)));

    [HasPermission(RecruitmentPermissions.ApprovePaidPosting)]
    [HttpPost("{requestId:guid}/job-postings/{postingId:guid}/reject-cost")]
    public async Task<ActionResult<ApiResponse<Unit>>> RejectPostingCost(
        Guid requestId, Guid postingId, [FromBody] RejectJobPostingCostCommand cmd, CancellationToken ct)
        => Ok(ApiResponse<Unit>.Ok(await sender.Send(
            cmd with { PostingId = postingId }, ct)));
}
