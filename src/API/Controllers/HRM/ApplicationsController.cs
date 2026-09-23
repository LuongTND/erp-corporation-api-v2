namespace API;

[Authorize]
[ApiController]
[Route("api/applications")]
public sealed class ApplicationsController(ISender sender) : ControllerBase
{
    [HasPermission(RecruitmentPermissions.ViewApplication)]
    [HttpGet("{applicationId:guid}")]
    public async Task<ActionResult<ApiResponse<ApplicationDetailResponse>>> GetById(
        Guid applicationId, CancellationToken ct)
        => Ok(ApiResponse<ApplicationDetailResponse>.Ok(
            await sender.Send(new GetApplicationDetailQuery(applicationId), ct)));

    [HasPermission(RecruitmentPermissions.CreateApplication)]
    [HttpPost]
    public async Task<ActionResult<ApiResponse<Guid>>> Create(
        [FromBody] CreateApplicationCommand cmd, CancellationToken ct)
        => Ok(ApiResponse<Guid>.Ok(await sender.Send(cmd, ct)));

    [HasPermission(RecruitmentPermissions.UpdateApplication)]
    [HttpDelete("{applicationId:guid}")]
    public async Task<ActionResult<ApiResponse<Unit>>> Delete(
        Guid applicationId, CancellationToken ct)
        => Ok(ApiResponse<Unit>.Ok(
            await sender.Send(new DeleteApplicationCommand(applicationId), ct)));
}
