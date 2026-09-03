namespace API;

[ApiController]
[Route("api/workflow/templates")]
public sealed class WorkflowTemplatesController(ISender sender) : ControllerBase
{
    [HasPermission(WorkflowPermissions.ManageTemplates)]
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<WorkflowTemplateResponse>>>> GetAll(
        [FromQuery] string? entityType, CancellationToken ct)
        => Ok(ApiResponse<IReadOnlyList<WorkflowTemplateResponse>>.Ok(
            await sender.Send(new GetWorkflowTemplatesQuery(entityType), ct)));

    [HasPermission(WorkflowPermissions.ManageTemplates)]
    [HttpPost]
    public async Task<ActionResult<ApiResponse<Guid>>> Create(
        [FromBody] CreateWorkflowTemplateCommand cmd, CancellationToken ct)
        => Ok(ApiResponse<Guid>.Ok(await sender.Send(cmd, ct)));

    [HasPermission(WorkflowPermissions.ManageTemplates)]
    [HttpDelete("{templateId:guid}")]
    public async Task<ActionResult<ApiResponse<Unit>>> Delete(Guid templateId, CancellationToken ct)
        => Ok(ApiResponse<Unit>.Ok(await sender.Send(new DeleteWorkflowTemplateCommand(templateId), ct)));

    [HasPermission(WorkflowPermissions.ManageTemplates)]
    [HttpPost("{templateId:guid}/steps")]
    public async Task<ActionResult<ApiResponse<Guid>>> AddStep(
        Guid templateId, [FromBody] AddWorkflowStepRequest req, CancellationToken ct)
        => Ok(ApiResponse<Guid>.Ok(await sender.Send(
            new AddWorkflowStepCommand(templateId, req.StepOrder, req.StepName, req.ApproverType, req.ApproverId), ct)));

    [HasPermission(WorkflowPermissions.ManageTemplates)]
    [HttpPut("{templateId:guid}/steps/{stepId:guid}")]
    public async Task<ActionResult<ApiResponse<Unit>>> UpdateStep(
        Guid templateId, Guid stepId, [FromBody] UpdateWorkflowStepRequest req, CancellationToken ct)
        => Ok(ApiResponse<Unit>.Ok(await sender.Send(
            new UpdateWorkflowStepCommand(templateId, stepId, req.StepName, req.ApproverType, req.ApproverId), ct)));

    [HasPermission(WorkflowPermissions.ManageTemplates)]
    [HttpDelete("{templateId:guid}/steps/{stepId:guid}")]
    public async Task<ActionResult<ApiResponse<Unit>>> DeleteStep(
        Guid templateId, Guid stepId, CancellationToken ct)
        => Ok(ApiResponse<Unit>.Ok(await sender.Send(new DeleteWorkflowStepCommand(templateId, stepId), ct)));
}

public sealed record AddWorkflowStepRequest(int StepOrder, string StepName, WorkflowApproverType ApproverType, Guid? ApproverId);
public sealed record UpdateWorkflowStepRequest(string StepName, WorkflowApproverType ApproverType, Guid? ApproverId);
