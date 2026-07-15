
namespace Application;
public sealed record UserScopeContext(
    ScopeType Scope,
    IReadOnlyList<Guid> AccessibleDepartmentIds);

public interface IDataScopeService
{
    Task<UserScopeContext> GetUserScopeContextAsync(Guid userId, CancellationToken ct = default);
    Task<ScopeType> GetEffectiveScopeAsync(Guid userId, CancellationToken ct = default);
    Task<IReadOnlyList<Guid>> GetAccessibleDepartmentIdsAsync(Guid userId, CancellationToken ct = default);
    Task<IQueryable<User>> ApplyUserScopeAsync(IQueryable<User> query, Guid userId, CancellationToken ct = default);
}
