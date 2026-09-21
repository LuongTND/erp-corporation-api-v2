namespace API;

[Authorize]
[ApiController]
[Route("api/recruitment-pipelines")]
public sealed class RecruitmentPipelinesController(ISender sender) : ControllerBase
{
    /// <summary>Lấy danh sách quy trình tuyển dụng</summary>
    [HasPermission(RecruitmentPermissions.ManageInterviewRule)]
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<RecruitmentPipelineResponse>>>> GetList(CancellationToken ct)
        => Ok(ApiResponse<IEnumerable<RecruitmentPipelineResponse>>.Ok(
            await sender.Send(new GetRecruitmentPipelinesQuery(), ct)));

    /// <summary>Tạo mới quy trình tuyển dụng</summary>
    [HasPermission(RecruitmentPermissions.ManageInterviewRule)]
    [HttpPost]
    public async Task<ActionResult<ApiResponse<Guid>>> Create(
        [FromBody] CreateRecruitmentPipelineCommand cmd, CancellationToken ct)
        => Ok(ApiResponse<Guid>.Ok(await sender.Send(cmd, ct)));

    /// <summary>Cập nhật tên / trạng thái mặc định của quy trình</summary>
    [HasPermission(RecruitmentPermissions.ManageInterviewRule)]
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResponse<Unit>>> Update(
        Guid id, [FromBody] UpdateRecruitmentPipelineCommand cmd, CancellationToken ct)
        => Ok(ApiResponse<Unit>.Ok(await sender.Send(cmd with { Id = id }, ct)));

    /// <summary>Xóa quy trình tuyển dụng (không xóa được quy trình mặc định)</summary>
    [HasPermission(RecruitmentPermissions.ManageInterviewRule)]
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ApiResponse<Unit>>> Delete(Guid id, CancellationToken ct)
        => Ok(ApiResponse<Unit>.Ok(await sender.Send(new DeleteRecruitmentPipelineCommand(id), ct)));

    /// <summary>Thêm vòng vào quy trình</summary>
    [HasPermission(RecruitmentPermissions.ManageInterviewRule)]
    [HttpPost("{id:guid}/stages")]
    public async Task<ActionResult<ApiResponse<Guid>>> AddStage(
        Guid id, [FromBody] AddPipelineStageCommand cmd, CancellationToken ct)
        => Ok(ApiResponse<Guid>.Ok(await sender.Send(cmd with { PipelineId = id }, ct)));

    /// <summary>Sửa tên vòng trong quy trình</summary>
    [HasPermission(RecruitmentPermissions.ManageInterviewRule)]
    [HttpPut("{id:guid}/stages/{stageId:guid}")]
    public async Task<ActionResult<ApiResponse<Unit>>> UpdateStage(
        Guid stageId, [FromBody] UpdatePipelineStageCommand cmd, CancellationToken ct)
        => Ok(ApiResponse<Unit>.Ok(await sender.Send(cmd with { StageId = stageId }, ct)));

    /// <summary>Xóa vòng khỏi quy trình (không xóa được vòng cố định)</summary>
    [HasPermission(RecruitmentPermissions.ManageInterviewRule)]
    [HttpDelete("{id:guid}/stages/{stageId:guid}")]
    public async Task<ActionResult<ApiResponse<Unit>>> RemoveStage(Guid stageId, CancellationToken ct)
        => Ok(ApiResponse<Unit>.Ok(await sender.Send(new RemovePipelineStageCommand(stageId), ct)));

    /// <summary>Sắp xếp lại thứ tự các vòng trong quy trình</summary>
    [HasPermission(RecruitmentPermissions.ManageInterviewRule)]
    [HttpPatch("{id:guid}/stages/reorder")]
    public async Task<ActionResult<ApiResponse<Unit>>> ReorderStages(
        Guid id, [FromBody] ReorderPipelineStagesCommand cmd, CancellationToken ct)
        => Ok(ApiResponse<Unit>.Ok(await sender.Send(cmd with { PipelineId = id }, ct)));
}
