using Domain.Entities;

namespace Application.Interfaces.Repositories.Users;

public interface IUserDepartmentRepository : IGenericRepository<UserDepartment>
{
    Task<List<UserDepartment>> GetSecondaryDepartmentsByUserIdAsync(Guid userId, CancellationToken ct = default);
    Task<bool> ExistsActiveSecondaryDepartmentAsync(Guid userId, Guid departmentId, CancellationToken ct = default);
    Task<UserDepartment?> GetActiveSecondaryDepartmentAsync(Guid userId, Guid departmentId, CancellationToken ct = default);
    Task<bool> HasActiveSecondaryUsersInDepartmentAsync(Guid departmentId, CancellationToken ct = default);
    Task<UserDepartment?> GetActivePrimaryDepartmentByUserIdAsync(Guid userId, CancellationToken ct = default);
    Task<List<UserDepartment>> GetActiveDepartmentsByUserIdAsync(Guid userId, CancellationToken ct = default);
}
