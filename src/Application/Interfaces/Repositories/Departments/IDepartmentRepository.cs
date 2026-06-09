using Domain.Entities;

namespace Application.Interfaces.Repositories.Departments;

public interface IDepartmentRepository : IGenericRepository<Department>
{
    Task<Department?> GetByIdWithDetailsAsync(Guid id, CancellationToken ct = default);
    Task<List<Department>> GetAllWithDetailsAsync(CancellationToken ct = default);
    Task<bool> ExistsByCodeAsync(string code, CancellationToken ct = default);
    Task<bool> ExistsByCodeExcludeIdAsync(string code, Guid excludeId, CancellationToken ct = default);
    Task<bool> HasActiveChildDepartmentsAsync(Guid departmentId, CancellationToken ct = default);
}
