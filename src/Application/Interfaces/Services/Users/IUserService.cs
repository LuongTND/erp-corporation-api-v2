using Application.Common.Models;
using Application.DTOs.Users;

namespace Application.Interfaces.Services.Users;

public interface IUserService
{
    Task<UserDto> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<PaginatedResult<UserDto>> GetPagedAsync(PaginationQuery query, CancellationToken ct = default);
    Task<UserDto> CreateAsync(CreateUserRequest request, CancellationToken ct = default);
    Task<UserDto> UpdateAsync(Guid id, UpdateUserRequest request, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default); // Soft delete or deactivate
    Task AssignRolesAsync(Guid id, List<Guid> roleIds, CancellationToken ct = default);
    Task ResetPasswordAsync(Guid id, string newPassword, CancellationToken ct = default);
    Task<IReadOnlyList<UserDepartmentDto>> GetSecondaryDepartmentsAsync(Guid userId, CancellationToken ct = default);
    Task<UserDepartmentDto> AddSecondaryDepartmentAsync(Guid userId, AddUserDepartmentRequest request, CancellationToken ct = default);
    Task RemoveSecondaryDepartmentAsync(Guid userId, Guid departmentId, CancellationToken ct = default);
}
