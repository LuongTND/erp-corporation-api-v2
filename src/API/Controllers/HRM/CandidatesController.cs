namespace API;

[Authorize]
[ApiController]
[Route("api/candidates")]
public sealed class CandidatesController(ISender sender) : ControllerBase
{
    [HasPermission(RecruitmentPermissions.CreateCandidate)]
    [HttpPost]
    public async Task<ActionResult<ApiResponse<Guid>>> Create(
        [FromBody] CreateCandidateCommand cmd, CancellationToken ct)
        => Ok(ApiResponse<Guid>.Ok(await sender.Send(cmd, ct)));

    [HasPermission(RecruitmentPermissions.ViewCandidate)]
    [HttpGet]
    public async Task<ActionResult<ApiResponse<QueryResult<CandidateResponse>>>> GetList(
        [FromQuery] QueryInfo queryInfo,
        [FromQuery] Guid? recruitmentRequestId,
        [FromQuery] ApplicationStage? stage,
        CancellationToken ct)
        => Ok(ApiResponse<QueryResult<CandidateResponse>>.Ok(
            await sender.Send(new GetCandidatesQuery(queryInfo, recruitmentRequestId, stage), ct)));

    [HasPermission(RecruitmentPermissions.ViewCandidate)]
    [HttpGet("{applicationId:guid}")]
    public async Task<ActionResult<ApiResponse<CandidateDetailResponse>>> GetById(
        Guid applicationId, CancellationToken ct)
        => Ok(ApiResponse<CandidateDetailResponse>.Ok(
            await sender.Send(new GetCandidateDetailQuery(applicationId), ct)));

    [HasPermission(RecruitmentPermissions.UpdateCandidate)]
    [HttpPut("{applicationId:guid}")]
    public async Task<ActionResult<ApiResponse<Unit>>> Update(
        Guid applicationId, [FromBody] UpdateCandidateCommand cmd, CancellationToken ct)
        => Ok(ApiResponse<Unit>.Ok(await sender.Send(cmd with { ApplicationId = applicationId }, ct)));

    [HasPermission(RecruitmentPermissions.UploadCv)]
    [HttpPost("{applicantId:guid}/cv")]
    public async Task<ActionResult<ApiResponse<string>>> UploadCv(
        Guid applicantId, IFormFile file, CancellationToken ct)
        => Ok(ApiResponse<string>.Ok(
            await sender.Send(new UploadCandidateCvCommand(applicantId, file.OpenReadStream(), file.FileName), ct)));

    [HasPermission(RecruitmentPermissions.ScreenCandidate)]
    [HttpPost("{applicationId:guid}/screen")]
    public async Task<ActionResult<ApiResponse<Unit>>> Screen(
        Guid applicationId, CancellationToken ct)
        => Ok(ApiResponse<Unit>.Ok(await sender.Send(new ScreenCandidateCommand(applicationId), ct)));

    [HasPermission(RecruitmentPermissions.AssignCandidate)]
    [HttpPost("{applicationId:guid}/assign-store")]
    public async Task<ActionResult<ApiResponse<Unit>>> AssignToStore(
        Guid applicationId, CancellationToken ct)
        => Ok(ApiResponse<Unit>.Ok(
            await sender.Send(new AssignCandidateToStoreCommand(applicationId), ct)));

    [HasPermission(RecruitmentPermissions.AssignCandidate)]
    [HttpPost("{applicationId:guid}/assign-production")]
    public async Task<ActionResult<ApiResponse<Unit>>> AssignToProduction(
        Guid applicationId, CancellationToken ct)
        => Ok(ApiResponse<Unit>.Ok(
            await sender.Send(new AssignCandidateToProductionCommand(applicationId), ct)));

    [HasPermission(RecruitmentPermissions.EvaluateCandidate)]
    [HttpPost("{applicationId:guid}/evaluations")]
    public async Task<ActionResult<ApiResponse<Guid>>> Evaluate(
        Guid applicationId, [FromBody] EvaluateCandidateCommand cmd, CancellationToken ct)
        => Ok(ApiResponse<Guid>.Ok(await sender.Send(cmd with { ApplicationId = applicationId }, ct)));

    [HasPermission(RecruitmentPermissions.ViewCandidate)]
    [HttpGet("{applicationId:guid}/evaluations")]
    public async Task<ActionResult<ApiResponse<QueryResult<CandidateEvaluationResponse>>>> GetEvaluations(
        Guid applicationId, CancellationToken ct)
        => Ok(ApiResponse<QueryResult<CandidateEvaluationResponse>>.Ok(
            await sender.Send(new GetCandidateEvaluationsQuery(applicationId), ct)));

    [HasPermission(RecruitmentPermissions.RejectCandidate)]
    [HttpPost("{applicationId:guid}/reject")]
    public async Task<ActionResult<ApiResponse<Unit>>> Reject(
        Guid applicationId, [FromBody] RejectCandidateCommand cmd, CancellationToken ct)
        => Ok(ApiResponse<Unit>.Ok(await sender.Send(cmd with { ApplicationId = applicationId }, ct)));

    [HasPermission(RecruitmentPermissions.HireCandidate)]
    [HttpPost("{applicationId:guid}/hire")]
    public async Task<ActionResult<ApiResponse<Unit>>> Hire(
        Guid applicationId, [FromBody] HireCandidateCommand cmd, CancellationToken ct)
        => Ok(ApiResponse<Unit>.Ok(await sender.Send(cmd with { ApplicationId = applicationId }, ct)));
}
