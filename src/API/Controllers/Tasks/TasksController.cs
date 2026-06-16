using API.Base;
using API.Filters;
using Application.Common.Models;
using Application.DTOs.Tasks;
using Application.Interfaces.Services.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Tasks;

public class TasksController : BaseApiController
{
    private readonly ITaskService _taskService;

    public TasksController(ITaskService taskService)
    {
        _taskService = taskService;
    }

    [HttpGet]
    [AuthorizePermission("task.item.read")]
    public async Task<ActionResult<PaginatedResult<TaskDto>>> GetPaged([FromQuery] TaskQuery query, CancellationToken ct)
    {
        var result = await _taskService.GetPagedAsync(query, ct);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [AuthorizePermission("task.item.read")]
    public async Task<ActionResult<TaskDto>> GetById(Guid id, CancellationToken ct)
    {
        var task = await _taskService.GetByIdAsync(id, ct);
        return Ok(task);
    }

    [HttpPost]
    [AuthorizePermission("task.item.create")]
    public async Task<ActionResult<TaskDto>> Create([FromBody] CreateTaskRequest request, CancellationToken ct)
    {
        var task = await _taskService.CreateAsync(request, ct);
        return CreatedAtAction(nameof(GetById), new { id = task.Id }, task);
    }

    [HttpPut("{id:guid}")]
    [AuthorizePermission("task.item.update")]
    public async Task<ActionResult<TaskDto>> Update(Guid id, [FromBody] UpdateTaskRequest request, CancellationToken ct)
    {
        var task = await _taskService.UpdateAsync(id, request, ct);
        return Ok(task);
    }

    [HttpDelete("{id:guid}")]
    [AuthorizePermission("task.item.delete")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await _taskService.DeleteAsync(id, ct);
        return NoContent();
    }

    [HttpGet("{id:guid}/comments")]
    [AuthorizePermission("task.item.read")]
    public async Task<ActionResult<IReadOnlyList<TaskCommentDto>>> GetComments(Guid id, CancellationToken ct)
    {
        var comments = await _taskService.GetCommentsAsync(id, ct);
        return Ok(comments);
    }

    [HttpPost("{id:guid}/comments")]
    [AuthorizePermission("task.comment.create")]
    public async Task<ActionResult<TaskCommentDto>> AddComment(Guid id, [FromBody] CreateCommentRequest request, CancellationToken ct)
    {
        var comment = await _taskService.AddCommentAsync(id, request, ct);
        return Ok(comment);
    }
}
