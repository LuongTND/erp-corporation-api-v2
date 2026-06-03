using Application.DTOs.Departments;

namespace Application.Interfaces.Services.Departments;

public interface IDepartmentService
{
    Task<DepartmentDto> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<DepartmentDto>> GetAllAsync(CancellationToken ct = default);
    Task<DepartmentDto> CreateAsync(CreateDepartmentRequest request, CancellationToken ct = default);
    Task<DepartmentDto> UpdateAsync(Guid id, UpdateDepartmentRequest request, CancellationToken ct = default);
}
