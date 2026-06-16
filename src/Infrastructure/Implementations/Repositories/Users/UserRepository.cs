using Application.Common.Models;
using Application.Interfaces.Repositories.Users;
using Infrastructure.Extensions;
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
            .Include(u => u.Manager)
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Id == id, ct);
    }

    public async Task<User?> GetByIdWithDetailsScopedAsync(Guid id, Guid currentUserId, ScopeType scope, IReadOnlyList<Guid> accessibleDeptIds, CancellationToken ct = default)
    {
        var query = DbSet
            .Include(u => u.Department)
            .Include(u => u.JobLevel)
            .Include(u => u.Manager)
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

    public async Task<PaginatedResult<User>> GetPagedWithDetailsScopedAsync(
        Guid currentUserId,
        ScopeType scope,
        IReadOnlyList<Guid> accessibleDeptIds,
        PaginationQuery query,
        CancellationToken ct = default)
    {
        var queryable = DbSet
            .Include(u => u.Department)
            .Include(u => u.JobLevel)
            .Include(u => u.Manager)
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .AsNoTracking();

        queryable = scope switch
        {
            ScopeType.Own => queryable.Where(u => u.Id == currentUserId),
            ScopeType.Team => queryable.Where(u => u.ManagerId == currentUserId || u.Id == currentUserId),
            ScopeType.Department => queryable.Where(u => accessibleDeptIds.Contains(u.DepartmentId)),
            ScopeType.All => queryable,
            _ => queryable.Where(u => u.Id == currentUserId)
        };

        return await queryable
            .OrderBy(u => u.EmployeeCode)
            .ToPaginatedResultAsync(query, ct);
    }

    public async Task<bool> ExistsByEmployeeCodeAsync(string employeeCode, CancellationToken ct = default)
    {
        var normalized = employeeCode.Trim();
        return await DbSet.AnyAsync(
            u => u.EmployeeCode.ToLower() == normalized.ToLower(),
            ct);
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

    public async Task<bool> HasActiveUsersWithJobLevelAsync(Guid jobLevelId, CancellationToken ct = default)
    {
        return await DbSet.AnyAsync(u => u.JobLevelId == jobLevelId && u.IsActive, ct);
    }

    public async Task<bool> HasActiveUsersInDepartmentAsync(Guid departmentId, CancellationToken ct = default)
    {
        return await DbSet.AnyAsync(u => u.DepartmentId == departmentId && u.IsActive, ct);
    }

    public async Task<User?> GetByIdWithAccountAsync(Guid id, CancellationToken ct = default)
    {
        return await DbSet
            .Include(u => u.UserAccount)
            .FirstOrDefaultAsync(u => u.Id == id, ct);
    }

    public async Task<User?> GetByIdWithAccountAndRolesAsync(Guid id, CancellationToken ct = default)
    {
        return await DbSet
            .Include(u => u.UserAccount)
            .Include(u => u.UserRoles)
            .FirstOrDefaultAsync(u => u.Id == id, ct);
    }

    public async Task AddUserRoleAsync(UserRole userRole, CancellationToken ct = default)
    {
        await Context.UserRoles.AddAsync(userRole, ct);
    }
}
