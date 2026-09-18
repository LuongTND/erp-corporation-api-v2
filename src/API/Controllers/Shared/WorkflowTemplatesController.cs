namespace API;

[ApiController]
[Route("api/workflow/templates")]
public sealed class WorkflowTemplatesController(ISender sender) : ControllerBase
{
    /// <summary>Lấy danh sách tất cả workflow template, có thể lọc theo loại phiếu (entityType)</summary>
    [HasPermission(WorkflowPermissions.ManageTemplates)]
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<WorkflowTemplateResponse>>>> GetAll(
        [FromQuery] string? entityType, CancellationToken ct)
        => Ok(ApiResponse<IReadOnlyList<WorkflowTemplateResponse>>.Ok(
            await sender.Send(new GetWorkflowTemplatesQuery(entityType), ct)));

    /// <summary>Lấy chi tiết một workflow template theo ID, kèm danh sách bước duyệt và tên người duyệt</summary>
    [HasPermission(WorkflowPermissions.ManageTemplates)]
    [HttpGet("{templateId:guid}")]
    public async Task<ActionResult<ApiResponse<WorkflowTemplateResponse>>> GetById(
        Guid templateId, CancellationToken ct)
        => Ok(ApiResponse<WorkflowTemplateResponse>.Ok(
            await sender.Send(new GetWorkflowTemplateDetailQuery(templateId), ct)));

    /// <summary>Tạo mới một workflow template (chưa có bước — thêm bước qua POST /steps)</summary>
    [HasPermission(WorkflowPermissions.ManageTemplates)]
    [HttpPost]
    public async Task<ActionResult<ApiResponse<Guid>>> Create(
        [FromBody] CreateWorkflowTemplateCommand cmd, CancellationToken ct)
        => Ok(ApiResponse<Guid>.Ok(await sender.Send(cmd, ct)));

    /// <summary>Lưu toàn bộ template sau khi chỉnh sửa: tên, scope, và danh sách bước duyệt (upsert + xóa bước bị bỏ)</summary>
    [HasPermission(WorkflowPermissions.ManageTemplates)]
    [HttpPut("{templateId:guid}")]
    public async Task<ActionResult<ApiResponse<Unit>>> Update(
        Guid templateId, [FromBody] UpdateWorkflowTemplateRequest req, CancellationToken ct)
        => Ok(ApiResponse<Unit>.Ok(await sender.Send(
            new UpdateWorkflowTemplateCommand(templateId, req.Name, req.ScopeType, req.ScopeEntityId, req.Steps), ct)));

    /// <summary>Bật/tắt trạng thái Active của template — khi tắt, engine sẽ không dùng template này nữa</summary>
    [HasPermission(WorkflowPermissions.ManageTemplates)]
    [HttpPatch("{templateId:guid}/toggle-active")]
    public async Task<ActionResult<ApiResponse<Unit>>> ToggleActive(Guid templateId, CancellationToken ct)
        => Ok(ApiResponse<Unit>.Ok(await sender.Send(new ToggleWorkflowTemplateActiveCommand(templateId), ct)));

    /// <summary>Xóa hẳn một workflow template (chỉ xóa được template chưa có workflow instance đang chạy)</summary>
    [HasPermission(WorkflowPermissions.ManageTemplates)]
    [HttpDelete("{templateId:guid}")]
    public async Task<ActionResult<ApiResponse<Unit>>> Delete(Guid templateId, CancellationToken ct)
        => Ok(ApiResponse<Unit>.Ok(await sender.Send(new DeleteWorkflowTemplateCommand(templateId), ct)));

    /// <summary>Thêm một bước duyệt mới vào template</summary>
    [HasPermission(WorkflowPermissions.ManageTemplates)]
    [HttpPost("{templateId:guid}/steps")]
    public async Task<ActionResult<ApiResponse<Guid>>> AddStep(
        Guid templateId, [FromBody] AddWorkflowStepRequest req, CancellationToken ct)
        => Ok(ApiResponse<Guid>.Ok(await sender.Send(
            new AddWorkflowStepCommand(templateId, req.StepOrder, req.StepName, req.ApproverType, req.ApproverId), ct)));

    /// <summary>Cập nhật thông tin một bước duyệt (tên, loại approver, người duyệt)</summary>
    [HasPermission(WorkflowPermissions.ManageTemplates)]
    [HttpPut("{templateId:guid}/steps/{stepId:guid}")]
    public async Task<ActionResult<ApiResponse<Unit>>> UpdateStep(
        Guid templateId, Guid stepId, [FromBody] UpdateWorkflowStepRequest req, CancellationToken ct)
        => Ok(ApiResponse<Unit>.Ok(await sender.Send(
            new UpdateWorkflowStepCommand(templateId, stepId, req.StepName, req.ApproverType, req.ApproverId), ct)));

    /// <summary>Cập nhật thứ tự các bước sau khi người dùng drag-and-drop — gửi lên toàn bộ danh sách với StepOrder mới</summary>
    [HasPermission(WorkflowPermissions.ManageTemplates)]
    [HttpPut("{templateId:guid}/steps/reorder")]
    public async Task<ActionResult<ApiResponse<Unit>>> ReorderSteps(
        Guid templateId, [FromBody] IReadOnlyList<StepOrderItem> items, CancellationToken ct)
        => Ok(ApiResponse<Unit>.Ok(await sender.Send(new ReorderWorkflowStepsCommand(templateId, items), ct)));

    /// <summary>Xóa một bước duyệt khỏi template</summary>
    [HasPermission(WorkflowPermissions.ManageTemplates)]
    [HttpDelete("{templateId:guid}/steps/{stepId:guid}")]
    public async Task<ActionResult<ApiResponse<Unit>>> DeleteStep(
        Guid templateId, Guid stepId, CancellationToken ct)
        => Ok(ApiResponse<Unit>.Ok(await sender.Send(new DeleteWorkflowStepCommand(templateId, stepId), ct)));
}

public sealed record UpdateWorkflowTemplateRequest(
    string? Name,                                   // null → không đổi tên
    WorkflowScopeType? ScopeType,                   // null → không đổi scope
    Guid? ScopeEntityId,                            // chỉ áp dụng khi ScopeType != null
    IReadOnlyList<UpdateWorkflowStepItem>? Steps    // null → không chạm vào steps
);
public sealed record AddWorkflowStepRequest(int StepOrder, string StepName, WorkflowApproverType ApproverType, Guid? ApproverId);
public sealed record UpdateWorkflowStepRequest(string StepName, WorkflowApproverType ApproverType, Guid? ApproverId);
