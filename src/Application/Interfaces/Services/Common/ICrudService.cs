namespace Application.Interfaces.Services.Common;

/// <summary>
/// Contract CRUD chuẩn: Repo (data) → Service (logic + map DTO) → API.
/// </summary>
public interface ICrudService<TDto, in TCreate, in TUpdate>
{
    Task<IReadOnlyList<TDto>> GetAllAsync(CancellationToken ct = default);
    Task<TDto> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<TDto> CreateAsync(TCreate request, CancellationToken ct = default);
    Task<TDto> UpdateAsync(Guid id, TUpdate request, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}
