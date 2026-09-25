namespace API;

[Authorize]
[ApiController]
[Route("api/labels")]
public sealed class LabelsController(ISender sender) : ControllerBase
{
    [HasPermission(LabelPermissions.View)]
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<LabelResponse>>>> GetLabels(
        [FromQuery] string? search, [FromQuery] bool? isActive, CancellationToken ct)
        => Ok(ApiResponse<IEnumerable<LabelResponse>>.Ok(
            await sender.Send(new GetLabelsQuery(search, isActive), ct)));

    [HasPermission(LabelPermissions.Manage)]
    [HttpPost]
    public async Task<ActionResult<ApiResponse<Guid>>> CreateLabel(
        [FromBody] CreateLabelCommand cmd, CancellationToken ct)
        => Ok(ApiResponse<Guid>.Ok(await sender.Send(cmd, ct)));

    [HasPermission(LabelPermissions.Manage)]
    [HttpPut("{labelId:guid}")]
    public async Task<ActionResult<ApiResponse<Unit>>> UpdateLabel(
        Guid labelId, [FromBody] UpdateLabelCommand cmd, CancellationToken ct)
        => Ok(ApiResponse<Unit>.Ok(await sender.Send(cmd with { LabelId = labelId }, ct)));

    [HasPermission(LabelPermissions.Manage)]
    [HttpDelete("{labelId:guid}")]
    public async Task<ActionResult<ApiResponse<Unit>>> DeleteLabel(
        Guid labelId, CancellationToken ct)
        => Ok(ApiResponse<Unit>.Ok(await sender.Send(new DeleteLabelCommand(labelId), ct)));
}
