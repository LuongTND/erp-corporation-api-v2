using Domain.Entities;
using Domain.Enums;

namespace Application.Interfaces.Services.Auth;

public interface IDataScopeService
{
    Task<ScopeType> GetEffectiveScopeAsync(Guid userId, CancellationToken ct = default);
    Task<IReadOnlyList<Guid>> GetAccessibleDepartmentIdsAsync(Guid userId, CancellationToken ct = default);
    Task<IQueryable<User>> ApplyUserScopeAsync(IQueryable<User> query, Guid userId, CancellationToken ct = default);
}
