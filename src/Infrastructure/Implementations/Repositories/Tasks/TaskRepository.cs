using Application.Interfaces.Repositories.Tasks;
using Domain.Entities.Tasks;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Implementations.Repositories.Tasks;

public class TaskRepository : GenericRepository<TaskItem>, ITaskRepository
{
    public TaskRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<TaskItem?> GetByIdWithDetailsAsync(Guid id, CancellationToken ct = default)
    {
        return await DbSet
            .AsNoTracking()
            .Include(t => t.Subtasks.Where(s => s.IsActive))
            .Include(t => t.Assignees)
                .ThenInclude(a => a.User)
            .Include(t => t.Followers)
                .ThenInclude(f => f.User)
            .Include(t => t.TaskKpis)
            .Include(t => t.TaskLmsCourses)
            .AsSplitQuery()
            .FirstOrDefaultAsync(t => t.Id == id, ct);
    }
}
