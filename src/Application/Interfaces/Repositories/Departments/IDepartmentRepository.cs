
namespace Application;
public interface IDepartmentRepository : IGenericRepository<Department>
{
    Task<Department?> GetByIdWithDetailsAsync(Guid id, CancellationToken ct = default);
    Task<PaginatedResult<Department>> GetPagedWithDetailsAsync(PaginationQuery query, CancellationToken ct = default);
    Task<bool> ExistsByCodeAsync(string code, CancellationToken ct = default);
    Task<bool> ExistsByCodeExcludeIdAsync(string code, Guid excludeId, CancellationToken ct = default);
    Task<bool> HasActiveChildDepartmentsAsync(Guid departmentId, CancellationToken ct = default);
    Task<List<(Guid Id, Guid? ParentDepartmentId)>> GetActiveHierarchyAsync(CancellationToken ct = default);
}
