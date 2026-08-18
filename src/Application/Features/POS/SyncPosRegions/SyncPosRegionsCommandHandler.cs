namespace Application;

public sealed class SyncPosRegionsCommandHandler(IUnitOfWork unitOfWork, IPosRegionReader posReader)
    : IRequestHandler<SyncPosRegionsCommand, int>
{
    public async Task<int> Handle(SyncPosRegionsCommand cmd, CancellationToken ct)
    {
        var posRegions = (await posReader.GetAllRegionsAsync(ct)).ToList();
        if (posRegions.Count == 0) return 0;

        var posIds = posRegions.Select(r => r.Id.ToString()).ToHashSet();

        var existing = (await unitOfWork.Repository<Region>()
            .GetAllTrackedAsync(r => posIds.Contains(r.PosRegionId), ct))
            .ToDictionary(r => r.PosRegionId);

        int added = 0;

        foreach (var pos in posRegions)
        {
            var posId = pos.Id.ToString();
            if (existing.TryGetValue(posId, out var region))
            {
                region.Name = pos.Name;
                region.Code = pos.Code;
                region.IsActive = pos.IsActive;
            }
            else
            {
                await unitOfWork.Repository<Region>().AddAsync(new Region
                {
                    Id = Guid.NewGuid(),
                    Name = pos.Name,
                    Code = pos.Code,
                    PosRegionId = posId,
                    IsActive = pos.IsActive,
                });
                added++;
            }
        }

        await unitOfWork.SaveChangesAsync(ct);
        return added;
    }
}
