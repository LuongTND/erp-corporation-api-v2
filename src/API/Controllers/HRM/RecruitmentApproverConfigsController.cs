namespace API;

[Authorize]
[ApiController]
[Route("api/recruitment/approver-configs")]
public sealed class RecruitmentApproverConfigsController(ISender sender) : ControllerBase
{
    [HasPermission(RecruitmentPermissions.ViewApproverConfig)]
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<RecruitmentApproverConfigResponse>>>> GetApprovers(
        CancellationToken ct)
        => Ok(ApiResponse<IEnumerable<RecruitmentApproverConfigResponse>>.Ok(
            await sender.Send(new GetRecruitmentApproversQuery(), ct)));

    [HasPermission(RecruitmentPermissions.ManageApproverConfig)]
    [HttpPost]
    public async Task<ActionResult<ApiResponse<Guid>>> SetApprover(
        [FromBody] SetRecruitmentApproverCommand cmd, CancellationToken ct)
        => Ok(ApiResponse<Guid>.Ok(await sender.Send(cmd, ct)));

    [HasPermission(RecruitmentPermissions.ManageApproverConfig)]
    [HttpDelete("{configId:guid}")]
    public async Task<ActionResult<ApiResponse<Unit>>> DeleteApprover(
        Guid configId, CancellationToken ct)
        => Ok(ApiResponse<Unit>.Ok(await sender.Send(new DeleteRecruitmentApproverCommand(configId), ct)));
}
