namespace Application;

public interface IDataScopeService
{
    Task<ScopeType> GetUserScopeAsync(Guid userId, CancellationToken ct = default);
    Task<IReadOnlySet<Guid>> GetAccessibleDepartmentIdsAsync(Guid userId, CancellationToken ct = default);
    Task<IQueryable<User>> ApplyScopeAsync(IQueryable<User> query, Guid userId, CancellationToken ct = default);
}
