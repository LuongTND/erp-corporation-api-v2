namespace Infrastructure;

[RegisterService(typeof(IStoreRepository))]
internal sealed class StoreRepository(ApplicationDbContext db) : IStoreRepository
{
    public Task<Store?> GetMyStoreAsync(Guid managerId, CancellationToken ct) =>
        db.Set<Store>()
            .AsSplitQuery()
            .Include(s => s.Region)
            .Include(s => s.Counters.Where(c => c.IsActive))
            .Include(s => s.StoreHours.Where(h => h.DayOfWeek == VietnamTime.Today))
            .Where(s => s.ManagerId == managerId && !s.IsDeleted)
            .FirstOrDefaultAsync(ct);
}
