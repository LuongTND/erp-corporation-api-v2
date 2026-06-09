using Application.Interfaces.Repositories.Departments;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Implementations.Repositories.Departments;

public class DepartmentRepository : GenericRepository<Department>, IDepartmentRepository
{
    public DepartmentRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<Department?> GetByIdWithDetailsAsync(Guid id, CancellationToken ct = default)
    {
        return await DbSet
            .Include(d => d.ParentDepartment)
            .Include(d => d.Manager)
            .FirstOrDefaultAsync(d => d.Id == id, ct);
    }

    public async Task<List<Department>> GetAllWithDetailsAsync(CancellationToken ct = default)
    {
        return await DbSet
            .Include(d => d.ParentDepartment)
            .Include(d => d.Manager)
            .ToListAsync(ct);
    }

    public async Task<bool> ExistsByCodeAsync(string code, CancellationToken ct = default)
    {
        return await DbSet.AnyAsync(d => d.DepartmentCode == code, ct);
    }

    public async Task<bool> ExistsByCodeExcludeIdAsync(string code, Guid excludeId, CancellationToken ct = default)
    {
        return await DbSet.AnyAsync(d => d.DepartmentCode == code && d.Id != excludeId, ct);
    }

    public async Task<bool> HasActiveChildDepartmentsAsync(Guid departmentId, CancellationToken ct = default)
    {
        return await DbSet.AnyAsync(d => d.ParentDepartmentId == departmentId && d.IsActive, ct);
    }
}
