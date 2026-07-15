
namespace Application;
public interface ITaskRepository : IGenericRepository<TaskItem>
{
    Task<TaskItem?> GetByIdWithDetailsAsync(Guid id, CancellationToken ct = default);
}
