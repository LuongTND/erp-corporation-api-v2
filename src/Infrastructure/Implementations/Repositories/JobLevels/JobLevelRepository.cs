using Application.Interfaces.Repositories.JobLevels;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Implementations.Repositories.JobLevels;

public class JobLevelRepository : GenericRepository<JobLevel>, IJobLevelRepository
{
    public JobLevelRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<bool> ExistsByNameAsync(string name, CancellationToken ct = default)
    {
        return await GetQueryable().AnyAsync(jl => jl.LevelName.ToLower() == name.ToLower(), ct);
    }

    public async Task<bool> ExistsByNameExcludeIdAsync(string name, Guid id, CancellationToken ct = default)
    {
        return await GetQueryable().AnyAsync(jl => jl.Id != id && jl.LevelName.ToLower() == name.ToLower(), ct);
    }
}
