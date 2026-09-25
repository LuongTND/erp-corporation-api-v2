namespace API;

[Authorize]
[ApiController]
[Route("api/recruitment-requests")]
public sealed class RecruitmentRequestsController(ISender sender) : ControllerBase
{
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

    [HasPermission(RecruitmentPermissions.SubmitRequest)]
    [HttpPost("{requestId:guid}/submit")]
    public async Task<ActionResult<ApiResponse<Unit>>> Submit(
        Guid requestId, CancellationToken ct)
        => Ok(ApiResponse<Unit>.Ok(await sender.Send(new SubmitRecruitmentRequestCommand(requestId), ct)));

    [Authorize]
    [HttpPost("{requestId:guid}/approve")]
    public async Task<ActionResult<ApiResponse<Unit>>> Approve(
        Guid requestId, [FromBody] ApproveRecruitmentRequestCommand cmd, CancellationToken ct)
        => Ok(ApiResponse<Unit>.Ok(await sender.Send(cmd with { RequestId = requestId }, ct)));

    [HasPermission(RecruitmentPermissions.RejectRequest)]
    [HttpPost("{requestId:guid}/reject")]
    public async Task<ActionResult<ApiResponse<Unit>>> Reject(
        Guid requestId, [FromBody] RejectRecruitmentRequestCommand cmd, CancellationToken ct)
        => Ok(ApiResponse<Unit>.Ok(await sender.Send(cmd with { RequestId = requestId }, ct)));

    [HasPermission(RecruitmentPermissions.RequestMoreInfo)]
    [HttpPost("{requestId:guid}/request-more-info")]
    public async Task<ActionResult<ApiResponse<Unit>>> RequestMoreInfo(
        Guid requestId, [FromBody] RequestMoreInfoRecruitmentCommand cmd, CancellationToken ct)
        => Ok(ApiResponse<Unit>.Ok(await sender.Send(cmd with { RequestId = requestId }, ct)));

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

}
