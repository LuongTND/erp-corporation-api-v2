
namespace Application;
public interface IJobLevelRepository : IGenericRepository<JobLevel>
{
    Task<bool> ExistsByNameAsync(string name, CancellationToken ct = default);
    Task<bool> ExistsByNameExcludeIdAsync(string name, Guid id, CancellationToken ct = default);
}
