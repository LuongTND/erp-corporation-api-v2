namespace Infrastructure;

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

    public async Task<PaginatedResult<Department>> GetPagedWithDetailsAsync(PaginationQuery query,
        CancellationToken ct = default)
    {
        return await DbSet
            .AsNoTracking()
            .Include(d => d.ParentDepartment)
            .Include(d => d.Manager)
            .OrderBy(d => d.DepartmentCode)
            .ToPaginatedResultAsync(query, ct);
    }

    public async Task<List<(Guid Id, Guid? ParentDepartmentId)>> GetActiveHierarchyAsync(CancellationToken ct = default)
    {
        return await DbSet
            .AsNoTracking()
            .Where(d => d.IsActive)
            .Select(d => new ValueTuple<Guid, Guid?>(d.Id, d.ParentDepartmentId))
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