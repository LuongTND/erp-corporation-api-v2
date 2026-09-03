namespace API;

[ApiController]
[Route("api/workflow")]
public sealed class WorkflowInstancesController(ISender sender) : ControllerBase
{
    [HttpGet("my-tasks")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<WorkflowTaskResponse>>>> GetMyTasks(
        [FromQuery] string? entityType, CancellationToken ct)
        => Ok(ApiResponse<IReadOnlyList<WorkflowTaskResponse>>.Ok(
            await sender.Send(new GetMyPendingTasksQuery(entityType), ct)));

    [HttpGet("instances/{instanceId:guid}/tasks")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<WorkflowTaskResponse>>>> GetInstanceTasks(
        Guid instanceId, CancellationToken ct)
        => Ok(ApiResponse<IReadOnlyList<WorkflowTaskResponse>>.Ok(
            await sender.Send(new GetInstanceTasksQuery(instanceId), ct)));
}
