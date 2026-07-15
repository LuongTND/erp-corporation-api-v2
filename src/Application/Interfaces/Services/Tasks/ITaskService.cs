
namespace Application;
public class TaskQuery : PaginationQuery
{
    public string? Search { get; set; }
    public string? Status { get; set; }
    public string? Priority { get; set; }
    public Guid? AssigneeId { get; set; }
}

public interface ITaskService
{
    Task<TaskDto> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<PaginatedResult<TaskDto>> GetPagedAsync(TaskQuery query, CancellationToken ct = default);
    Task<TaskDto> CreateAsync(CreateTaskRequest request, CancellationToken ct = default);
    Task<TaskDto> UpdateAsync(Guid id, UpdateTaskRequest request, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
    Task<TaskCommentDto> AddCommentAsync(Guid taskId, CreateCommentRequest request, CancellationToken ct = default);
    Task<IReadOnlyList<TaskCommentDto>> GetCommentsAsync(Guid taskId, CancellationToken ct = default);
}
