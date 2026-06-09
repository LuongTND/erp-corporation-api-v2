using Application.Interfaces.Repositories.Users;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Implementations.Repositories.Users;

public class UserDepartmentRepository : GenericRepository<UserDepartment>, IUserDepartmentRepository
{
    public UserDepartmentRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<List<UserDepartment>> GetSecondaryDepartmentsByUserIdAsync(Guid userId, CancellationToken ct = default)
    {
        return await GetQueryable()
            .Include(ud => ud.Department)
            .Where(ud => ud.UserId == userId && !ud.IsPrimary && ud.IsActive)
            .ToListAsync(ct);
    }

    public async Task<bool> ExistsActiveSecondaryDepartmentAsync(Guid userId, Guid departmentId, CancellationToken ct = default)
    {
        return await GetQueryable()
            .AnyAsync(ud => ud.UserId == userId && ud.DepartmentId == departmentId && ud.IsActive, ct);
    }

    public async Task<UserDepartment?> GetActiveSecondaryDepartmentAsync(Guid userId, Guid departmentId, CancellationToken ct = default)
    {
        return await GetQueryable()
            .FirstOrDefaultAsync(ud => ud.UserId == userId && ud.DepartmentId == departmentId && !ud.IsPrimary && ud.IsActive, ct);
    }

    public async Task<bool> HasActiveSecondaryUsersInDepartmentAsync(Guid departmentId, CancellationToken ct = default)
    {
        return await GetQueryable()
            .AnyAsync(ud => ud.DepartmentId == departmentId && ud.IsActive, ct);
    }

    public async Task<UserDepartment?> GetActivePrimaryDepartmentByUserIdAsync(Guid userId, CancellationToken ct = default)
    {
        return await GetQueryable()
            .FirstOrDefaultAsync(ud => ud.UserId == userId && ud.IsPrimary && ud.IsActive, ct);
    }

    public async Task<List<UserDepartment>> GetActiveDepartmentsByUserIdAsync(Guid userId, CancellationToken ct = default)
    {
        return await GetQueryable()
            .Where(ud => ud.UserId == userId && ud.IsActive)
            .ToListAsync(ct);
    }
}
