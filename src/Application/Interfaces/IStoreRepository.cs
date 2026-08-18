namespace Application;

public interface IStoreRepository
{
    Task<Store?> GetMyStoreAsync(Guid managerId, CancellationToken ct);
}
