namespace Infrastructure;

[RegisterService(typeof(IUnitOfWork))]
public sealed class UnitOfWork(ApplicationDbContext db) : IUnitOfWork
{
    private readonly Dictionary<Type, object> _repos = [];
    private IDbContextTransaction? _transaction;

    public IGenericRepository<T> Repository<T>() where T : EntityBase<Guid>
    {
        if (!_repos.TryGetValue(typeof(T), out var repo))
            _repos[typeof(T)] = repo = new GenericRepository<T>(db);
        return (IGenericRepository<T>)repo;
    }

    public Task<int> SaveChangesAsync(CancellationToken ct = default)
        => db.SaveChangesAsync(ct);

    public async Task BeginTransactionAsync(CancellationToken ct = default)
        => _transaction = await db.Database.BeginTransactionAsync(ct);

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
