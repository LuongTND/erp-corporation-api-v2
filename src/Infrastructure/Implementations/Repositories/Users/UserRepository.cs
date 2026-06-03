using Application.Interfaces.Repositories.Users;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Implementations.Repositories.Users;

public class UserRepository : GenericRepository<User>, IUserRepository
{
    public UserRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<User?> GetByIdWithDetailsAsync(Guid id, CancellationToken ct = default)
    {
        return await DbSet
            .Include(u => u.Department)
            .Include(u => u.JobLevel)
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Id == id, ct);
    }

    public async Task<User?> GetByIdWithDetailsScopedAsync(Guid id, Guid currentUserId, ScopeType scope, IReadOnlyList<Guid> accessibleDeptIds, CancellationToken ct = default)
    {
        var query = DbSet
            .Include(u => u.Department)
            .Include(u => u.JobLevel)
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .Where(u => u.Id == id);

        query = scope switch
        {
            ScopeType.Own => query.Where(u => u.Id == currentUserId),
            ScopeType.Team => query.Where(u => u.ManagerId == currentUserId || u.Id == currentUserId),
            ScopeType.Department => query.Where(u => accessibleDeptIds.Contains(u.DepartmentId)),
            ScopeType.All => query,
            _ => query.Where(u => u.Id == currentUserId)
        };

        return await query.FirstOrDefaultAsync(ct);
    }

    public async Task<List<User>> GetWithDetailsScopedAsync(Guid currentUserId, ScopeType scope, IReadOnlyList<Guid> accessibleDeptIds, CancellationToken ct = default)
    {
        var query = DbSet
            .Include(u => u.Department)
            .Include(u => u.JobLevel)
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .AsNoTracking();

        query = scope switch
        {
            ScopeType.Own => query.Where(u => u.Id == currentUserId),
            ScopeType.Team => query.Where(u => u.ManagerId == currentUserId || u.Id == currentUserId),
            ScopeType.Department => query.Where(u => accessibleDeptIds.Contains(u.DepartmentId)),
            ScopeType.All => query,
            _ => query.Where(u => u.Id == currentUserId)
        };

        return await query.ToListAsync(ct);
    }

    public async Task<bool> ExistsByEmployeeCodeAsync(string employeeCode, CancellationToken ct = default)
    {
        return await DbSet.AnyAsync(u => u.EmployeeCode == employeeCode, ct);
    }

    public async Task<bool> ExistsByEmailAsync(string email, CancellationToken ct = default)
    {
        return await DbSet.AnyAsync(u => u.Email == email, ct);
    }

    public async Task<bool> ExistsByEmailExcludeIdAsync(string email, Guid excludeId, CancellationToken ct = default)
    {
        return await DbSet.AnyAsync(u => u.Email == email && u.Id != excludeId, ct);
    }

    public async Task<User?> GetByIdWithRolesAsync(Guid id, CancellationToken ct = default)
    {
        return await DbSet
            .Include(u => u.UserRoles)
            .FirstOrDefaultAsync(u => u.Id == id, ct);
    }

    public async Task<User?> GetJobLevelScopeInfoAsync(Guid id, CancellationToken ct = default)
    {
        return await DbSet
            .Include(u => u.JobLevel)
            .FirstOrDefaultAsync(u => u.Id == id, ct);
    }
}
