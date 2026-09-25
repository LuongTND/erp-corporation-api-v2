namespace Application;

public sealed class SyncPosStoresCommandHandler(IUnitOfWork unitOfWork, IPosStoreReader posReader)
    : IRequestHandler<SyncPosStoresCommand, int>
{
    public async Task<int> Handle(SyncPosStoresCommand cmd, CancellationToken ct)
    {
        var posStores = (await posReader.GetAllStoresAsync(ct)).ToList();
        if (posStores.Count == 0) return 0;

        var posIds = posStores.Select(p => p.Id.ToString()).ToHashSet();

        var existing = (await unitOfWork.Repository<Store>()
            .GetAllTrackedAsync(s => posIds.Contains(s.PosStoreId), ct))
            .ToDictionary(s => s.PosStoreId);

        // Build posRegionId → HRM Region.Id map for FK resolution
        var posRegionIds = posStores
            .Where(p => p.RegionId.HasValue)
            .Select(p => p.RegionId!.Value.ToString())
            .ToHashSet();
        var regionMap = posRegionIds.Count == 0
            ? new Dictionary<string, Guid>()
            : (await unitOfWork.Repository<Region>()
                .GetAllTrackedAsync(r => posRegionIds.Contains(r.PosRegionId), ct))
                .ToDictionary(r => r.PosRegionId, r => r.Id);

        int added = 0;

        foreach (var pos in posStores)
        {
            var posId = pos.Id.ToString();
            var regionId = pos.RegionId.HasValue && regionMap.TryGetValue(pos.RegionId.Value.ToString(), out var rid)
                ? rid : (Guid?)null;

            if (existing.TryGetValue(posId, out var store))
            {
                store.Name = pos.Name;
                store.Address = pos.Address;
                store.Phone = pos.Phone;
                store.RegionId = regionId;
            }
            else
            {
                await unitOfWork.Repository<Store>().AddAsync(new Store
                {
                    Id = Guid.NewGuid(),
                    Name = pos.Name,
                    Code = $"STORE-{posId[..8].ToUpperInvariant()}",
                    PosStoreId = posId,
                    Address = pos.Address,
                    Phone = pos.Phone,
                    IsActive = true,
                    RegionId = regionId,
                });
                added++;
            }
        }

        await unitOfWork.SaveChangesAsync(ct);
        return added;
    }
}
