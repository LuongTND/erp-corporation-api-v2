using System.Linq.Expressions;

namespace Infrastructure;

public class GenericRepository<T> : IGenericRepository<T> where T : EntityBase<Guid>
{
    private readonly ApplicationDbContext _db;
    private readonly DbSet<T> _dbSet;

    public GenericRepository(ApplicationDbContext db)
    {
        _db = db;
        _dbSet = db.Set<T>();
    }

    public async Task<T> AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        return entity;
    }

    public Task RemoveAsync(T entity)
    {
        _dbSet.Remove(entity);
        return Task.CompletedTask;
    }

    public Task RemoveRangeAsync(IEnumerable<T> entities)
    {
        _dbSet.RemoveRange(entities);
        return Task.CompletedTask;
    }

    public async Task<T?> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default)
        => await _dbSet.AsNoTracking().FirstOrDefaultAsync(predicate, ct);

    public async Task<T?> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken ct, params Expression<Func<T, object>>[] includes)
    {
        var query = includes.Aggregate(
            _dbSet.AsNoTracking().Where(predicate),
            (q, include) => q.Include(include));
        return await query.FirstOrDefaultAsync(ct);
    }

    public async Task<T?> FindTrackedAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default)
        => await _dbSet.FirstOrDefaultAsync(predicate, ct);

    public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default)
        => await _dbSet.AnyAsync(predicate, ct);

    public IQueryable<T> Query(bool tracking = false)
        => tracking ? _dbSet : _dbSet.AsNoTracking();

    public async Task<QueryResult<T>> GetPagedAsync(
        QueryInfo query,
        Expression<Func<T, bool>>? filter = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        CancellationToken ct = default)
    {
        var q = _dbSet.AsNoTracking();

        if (filter != null) q = q.Where(filter);

        var total = query.NeedTotalCount ? await q.CountAsync(ct) : 0;

        if (orderBy != null) q = orderBy(q);

        var items = await q.Skip(query.Skip).Take(Math.Min(query.Top, AppConstants.MaxPageSize)).ToListAsync(ct);

        return new QueryResult<T> { Items = items, TotalCount = total };
    }
}
