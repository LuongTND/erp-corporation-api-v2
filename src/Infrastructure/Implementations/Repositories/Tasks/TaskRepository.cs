namespace Infrastructure;

public class TaskRepository : GenericRepository<TaskItem>, ITaskRepository
{
    public TaskRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<TaskItem?> GetByIdWithDetailsAsync(Guid id, CancellationToken ct = default)
    {
        return await DbSet
            .Include(t => t.Subtasks)
            .Include(t => t.Assignees)
            .ThenInclude(a => a.User)
            .Include(t => t.Followers)
            .ThenInclude(f => f.User)
            .Include(t => t.Comments)
            .ThenInclude(c => c.User)
            .Include(t => t.ActivityLogs)
            .ThenInclude(l => l.User)
            .Include(t => t.TaskKpis)
            .Include(t => t.TaskLmsCourses)
            .FirstOrDefaultAsync(t => t.Id == id, ct);
    }
}