using Domain.Entities.Tasks;

namespace Application.Interfaces.Repositories.Tasks;

public interface ITaskRepository : IGenericRepository<TaskItem>
{
    Task<TaskItem?> GetByIdWithDetailsAsync(Guid id, CancellationToken ct = default);
}
