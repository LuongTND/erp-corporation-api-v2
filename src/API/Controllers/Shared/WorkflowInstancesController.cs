namespace API;

[ApiController]
[Route("api/workflow")]
public sealed class WorkflowInstancesController(ISender sender) : ControllerBase
{
    /// <summary>Lấy danh sách task đang chờ tôi duyệt.</summary>
    [HasPermission(WorkflowPermissions.ViewMyTasks)]
    [HttpGet("my-tasks")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<WorkflowTaskResponse>>>> GetMyTasks(
        [FromQuery] string? entityType, CancellationToken ct)
        => Ok(ApiResponse<IReadOnlyList<WorkflowTaskResponse>>.Ok(
            await sender.Send(new GetMyPendingTasksQuery(entityType), ct)));

    /// <summary>Lấy toàn bộ lịch sử task của một workflow instance.</summary>
    [HasPermission(WorkflowPermissions.ViewInstanceTasks)]
    [HttpGet("instances/{instanceId:guid}/tasks")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<WorkflowTaskResponse>>>> GetInstanceTasks(
        Guid instanceId, CancellationToken ct)
        => Ok(ApiResponse<IReadOnlyList<WorkflowTaskResponse>>.Ok(
            await sender.Send(new GetInstanceTasksQuery(instanceId), ct)));

    /// <summary>Hủy một workflow instance đang InProgress (chỉ người tạo mới được hủy).</summary>
    [HasPermission(WorkflowPermissions.ViewMyTasks)]
    [HttpPost("instances/{instanceId:guid}/cancel")]
    public async Task<ActionResult<ApiResponse<Unit>>> CancelInstance(
        Guid instanceId, [FromServices] IUserContext userContext, CancellationToken ct)
    {
        await sender.Send(new CancelWorkflowInstanceCommand(instanceId, userContext.UserId), ct);
        return Ok(ApiResponse<Unit>.Ok(Unit.Value));
    }

    /// <summary>Lấy danh sách loại đối tượng hỗ trợ workflow (dùng cho dropdown tạo template).</summary>
    [HasPermission(WorkflowPermissions.ManageTemplates)]
    [HttpGet("entity-types")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<WorkflowEntityTypeItem>>>> GetEntityTypes(CancellationToken ct)
        => Ok(ApiResponse<IReadOnlyList<WorkflowEntityTypeItem>>.Ok(
            await sender.Send(new GetWorkflowEntityTypesQuery(), ct)));

    /// <summary>Lấy danh sách phạm vi áp dụng workflow (dùng cho dropdown tạo template).</summary>
    [HasPermission(WorkflowPermissions.ManageTemplates)]
    [HttpGet("scope-types")]
    public ActionResult<ApiResponse<IReadOnlyList<WorkflowScopeTypeItem>>> GetScopeTypes()
        => Ok(ApiResponse<IReadOnlyList<WorkflowScopeTypeItem>>.Ok(WorkflowScopeTypes.All));

    /// <summary>Lấy danh sách loại người duyệt workflow (dùng cho dropdown tạo bước).</summary>
    [HasPermission(WorkflowPermissions.ManageTemplates)]
    [HttpGet("approver-types")]
    public ActionResult<ApiResponse<IReadOnlyList<WorkflowApproverTypeItem>>> GetApproverTypes()
        => Ok(ApiResponse<IReadOnlyList<WorkflowApproverTypeItem>>.Ok(WorkflowApproverTypes.All));
}
