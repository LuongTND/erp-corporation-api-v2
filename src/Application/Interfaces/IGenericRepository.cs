using System.Linq.Expressions;

namespace Application;

public interface IGenericRepository<T> where T : EntityBase<Guid>
{
    Task<T> AddAsync(T entity);
    Task RemoveAsync(T entity);
    Task RemoveRangeAsync(IEnumerable<T> entities);
    Task<T?> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default);
    Task<T?> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken ct, params Expression<Func<T, object>>[] includes);
    Task<T?> FindTrackedAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default);
    Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default);
    Task<QueryResult<T>> GetPagedAsync(
        QueryInfo query,
        Expression<Func<T, bool>>? filter = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        CancellationToken ct = default);
    IQueryable<T> Query(bool tracking = false);
}
