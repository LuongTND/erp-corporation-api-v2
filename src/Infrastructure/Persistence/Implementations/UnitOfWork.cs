using Microsoft.EntityFrameworkCore.Storage;

namespace Infrastructure;

[RegisterService(typeof(IUnitOfWork))]
public sealed class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _db;
    private readonly Dictionary<string, object> _repos = [];
    private IDbContextTransaction? _transaction;

    public UnitOfWork(ApplicationDbContext db) => _db = db;

    public IGenericRepository<T> Repository<T>() where T : EntityBase<Guid>
    {
        var key = typeof(T).Name;
        if (!_repos.TryGetValue(key, out var repo))
            _repos[key] = repo = new GenericRepository<T>(_db);
        return (IGenericRepository<T>)repo;
    }

    public Task<int> SaveChangesAsync(CancellationToken ct = default)
        => _db.SaveChangesAsync(ct);

    public async Task BeginTransactionAsync(CancellationToken ct = default)
        => _transaction = await _db.Database.BeginTransactionAsync(ct);

    public async Task CommitTransactionAsync()
    {
        if (_transaction is null) return;
        await _transaction.CommitAsync();
        await _transaction.DisposeAsync();
        _transaction = null;
    }

    public async Task RollbackTransactionAsync(CancellationToken ct = default)
    {
        if (_transaction is null) return;
        await _transaction.RollbackAsync(ct);
        await _transaction.DisposeAsync();
        _transaction = null;
    }

    public void Dispose() => _transaction?.Dispose();
}
