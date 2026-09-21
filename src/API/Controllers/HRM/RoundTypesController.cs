namespace API;

[Authorize]
[ApiController]
[Route("api/round-types")]
public sealed class RoundTypesController(ISender sender) : ControllerBase
{
    /// <summary>Lấy danh sách loại vòng tuyển dụng</summary>
    [HasPermission(RecruitmentPermissions.ManageInterviewRule)]
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<RoundTypeResponse>>>> GetList(CancellationToken ct)
        => Ok(ApiResponse<IEnumerable<RoundTypeResponse>>.Ok(await sender.Send(new GetRoundTypesQuery(), ct)));

    /// <summary>Tạo mới loại vòng tuyển dụng</summary>
    [HasPermission(RecruitmentPermissions.ManageInterviewRule)]
    [HttpPost]
    public async Task<ActionResult<ApiResponse<Guid>>> Create(
        [FromBody] CreateRoundTypeCommand cmd, CancellationToken ct)
        => Ok(ApiResponse<Guid>.Ok(await sender.Send(cmd, ct)));

    /// <summary>Cập nhật loại vòng tuyển dụng</summary>
    [HasPermission(RecruitmentPermissions.ManageInterviewRule)]
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResponse<Unit>>> Update(
        Guid id, [FromBody] UpdateRoundTypeCommand cmd, CancellationToken ct)
        => Ok(ApiResponse<Unit>.Ok(await sender.Send(cmd with { Id = id }, ct)));

    /// <summary>Cập nhật thứ tự hàng loạt (1 request thay vì N PUT)</summary>
    [HasPermission(RecruitmentPermissions.ManageInterviewRule)]
    [HttpPatch("reorder")]
    public async Task<ActionResult<ApiResponse<Unit>>> Reorder(
        [FromBody] ReorderRoundTypesCommand cmd, CancellationToken ct)
        => Ok(ApiResponse<Unit>.Ok(await sender.Send(cmd, ct)));

    /// <summary>Xoá loại vòng tuyển dụng (không xoá được loại mặc định hệ thống)</summary>
    [HasPermission(RecruitmentPermissions.ManageInterviewRule)]
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ApiResponse<Unit>>> Delete(Guid id, CancellationToken ct)
        => Ok(ApiResponse<Unit>.Ok(await sender.Send(new DeleteRoundTypeCommand(id), ct)));
}
