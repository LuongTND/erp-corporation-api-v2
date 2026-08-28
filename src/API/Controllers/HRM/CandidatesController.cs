namespace API;

[Authorize]
[ApiController]
[Route("api/candidates")]
public sealed class CandidatesController(ISender sender) : ControllerBase
{
    // HRM-059
    [HasPermission(RecruitmentPermissions.CreateCandidate)]
    [HttpPost]
    public async Task<ActionResult<ApiResponse<Guid>>> Create(
        [FromBody] CreateCandidateCommand cmd, CancellationToken ct)
        => Ok(ApiResponse<Guid>.Ok(await sender.Send(cmd, ct)));

    // HRM-068: pipeline
    [HasPermission(RecruitmentPermissions.ViewCandidate)]
    [HttpGet]
    public async Task<ActionResult<ApiResponse<QueryResult<CandidateResponse>>>> GetList(
        [FromQuery] QueryInfo queryInfo,
        [FromQuery] Guid? recruitmentRequestId,
        [FromQuery] CandidateStage? stage,
        CancellationToken ct)
        => Ok(ApiResponse<QueryResult<CandidateResponse>>.Ok(
            await sender.Send(new GetCandidatesQuery(queryInfo, recruitmentRequestId, stage), ct)));

    [HasPermission(RecruitmentPermissions.ViewCandidate)]
    [HttpGet("{candidateId:guid}")]
    public async Task<ActionResult<ApiResponse<CandidateDetailResponse>>> GetById(
        Guid candidateId, CancellationToken ct)
        => Ok(ApiResponse<CandidateDetailResponse>.Ok(
            await sender.Send(new GetCandidateDetailQuery(candidateId), ct)));

    [HasPermission(RecruitmentPermissions.UpdateCandidate)]
    [HttpPut("{candidateId:guid}")]
    public async Task<ActionResult<ApiResponse<Unit>>> Update(
        Guid candidateId, [FromBody] UpdateCandidateCommand cmd, CancellationToken ct)
        => Ok(ApiResponse<Unit>.Ok(await sender.Send(cmd with { CandidateId = candidateId }, ct)));

    // HRM-060: upload CV
    [HasPermission(RecruitmentPermissions.UploadCv)]
    [HttpPost("{candidateId:guid}/cv")]
    public async Task<ActionResult<ApiResponse<string>>> UploadCv(
        Guid candidateId, IFormFile file, CancellationToken ct)
        => Ok(ApiResponse<string>.Ok(
            await sender.Send(new UploadCandidateCvCommand(candidateId, file.OpenReadStream(), file.FileName), ct)));

    // HRM-061: sơ loại
    [HasPermission(RecruitmentPermissions.ScreenCandidate)]
    [HttpPost("{candidateId:guid}/screen")]
    public async Task<ActionResult<ApiResponse<Unit>>> Screen(
        Guid candidateId, CancellationToken ct)
        => Ok(ApiResponse<Unit>.Ok(await sender.Send(new ScreenCandidateCommand(candidateId), ct)));

    // HRM-062
    [HasPermission(RecruitmentPermissions.AssignCandidate)]
    [HttpPost("{candidateId:guid}/assign-store")]
    public async Task<ActionResult<ApiResponse<Unit>>> AssignToStore(
        Guid candidateId, CancellationToken ct)
        => Ok(ApiResponse<Unit>.Ok(
            await sender.Send(new AssignCandidateToStoreCommand(candidateId), ct)));

    // HRM-063
    [HasPermission(RecruitmentPermissions.AssignCandidate)]
    [HttpPost("{candidateId:guid}/assign-production")]
    public async Task<ActionResult<ApiResponse<Unit>>> AssignToProduction(
        Guid candidateId, CancellationToken ct)
        => Ok(ApiResponse<Unit>.Ok(
            await sender.Send(new AssignCandidateToProductionCommand(candidateId), ct)));

    // HRM-064 / HRM-065
    [HasPermission(RecruitmentPermissions.EvaluateCandidate)]
    [HttpPost("{candidateId:guid}/evaluations")]
    public async Task<ActionResult<ApiResponse<Guid>>> Evaluate(
        Guid candidateId, [FromBody] EvaluateCandidateCommand cmd, CancellationToken ct)
        => Ok(ApiResponse<Guid>.Ok(await sender.Send(cmd with { CandidateId = candidateId }, ct)));

    [HasPermission(RecruitmentPermissions.ViewCandidate)]
    [HttpGet("{candidateId:guid}/evaluations")]
    public async Task<ActionResult<ApiResponse<QueryResult<CandidateEvaluationResponse>>>> GetEvaluations(
        Guid candidateId, CancellationToken ct)
        => Ok(ApiResponse<QueryResult<CandidateEvaluationResponse>>.Ok(
            await sender.Send(new GetCandidateEvaluationsQuery(candidateId), ct)));

    // HRM-066
    [HasPermission(RecruitmentPermissions.RejectCandidate)]
    [HttpPost("{candidateId:guid}/reject")]
    public async Task<ActionResult<ApiResponse<Unit>>> Reject(
        Guid candidateId, [FromBody] RejectCandidateCommand cmd, CancellationToken ct)
        => Ok(ApiResponse<Unit>.Ok(await sender.Send(cmd with { CandidateId = candidateId }, ct)));

    // HRM-067
    [HasPermission(RecruitmentPermissions.HireCandidate)]
    [HttpPost("{candidateId:guid}/hire")]
    public async Task<ActionResult<ApiResponse<Unit>>> Hire(
        Guid candidateId, [FromBody] HireCandidateCommand cmd, CancellationToken ct)
        => Ok(ApiResponse<Unit>.Ok(await sender.Send(cmd with { CandidateId = candidateId }, ct)));
}
