namespace Application;

public sealed class UpsertRegionHoursCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<UpsertRegionHoursCommand, Unit>
{
    public async Task<Unit> Handle(UpsertRegionHoursCommand cmd, CancellationToken ct)
    {
        var regionExists = await unitOfWork.Repository<Region>()
            .AnyAsync(r => r.Id == cmd.RegionId && !r.IsDeleted, ct);
        if (!regionExists) throw new NotFoundException(ExceptionMessages.NotFound("Region", cmd.RegionId));

        var days = cmd.Hours.Select(h => h.DayOfWeek).ToList();

        var existing = (await unitOfWork.Repository<RegionHours>()
            .GetAllTrackedAsync(h => h.RegionId == cmd.RegionId && days.Contains(h.DayOfWeek), ct))
            .ToDictionary(h => h.DayOfWeek);

        foreach (var item in cmd.Hours)
        {
            if (existing.TryGetValue(item.DayOfWeek, out var row))
            {
                row.OpenTime = item.OpenTime;
                row.CloseTime = item.CloseTime;
                row.IsClosed = item.IsClosed;
            }
            else
            {
                await unitOfWork.Repository<RegionHours>().AddAsync(new RegionHours
                {
                    Id = Guid.NewGuid(),
                    RegionId = cmd.RegionId,
                    DayOfWeek = item.DayOfWeek,
                    OpenTime = item.OpenTime,
                    CloseTime = item.CloseTime,
                    IsClosed = item.IsClosed,
                });
            }
        }

        // Propagate to all stores in this region
        var storeIds = await unitOfWork.Repository<Store>()
            .Query()
            .Where(s => s.RegionId == cmd.RegionId && !s.IsDeleted)
            .Select(s => s.Id)
            .ToListAsync(ct);

        if (storeIds.Count > 0)
        {
            var existingStoreHours = (await unitOfWork.Repository<StoreHours>()
                .GetAllTrackedAsync(h => storeIds.Contains(h.StoreId) && days.Contains(h.DayOfWeek), ct))
                .GroupBy(h => h.StoreId)
                .ToDictionary(g => g.Key, g => g.ToDictionary(h => h.DayOfWeek));

            foreach (var storeId in storeIds)
            {
                existingStoreHours.TryGetValue(storeId, out var storeExisting);
                foreach (var item in cmd.Hours)
                {
                    if (storeExisting != null && storeExisting.TryGetValue(item.DayOfWeek, out var sh))
                    {
                        sh.OpenTime = item.OpenTime;
                        sh.CloseTime = item.CloseTime;
                        sh.IsClosed = item.IsClosed;
                    }
                    else
                    {
                        await unitOfWork.Repository<StoreHours>().AddAsync(new StoreHours
                        {
                            Id = Guid.NewGuid(),
                            StoreId = storeId,
                            DayOfWeek = item.DayOfWeek,
                            OpenTime = item.OpenTime,
                            CloseTime = item.CloseTime,
                            IsClosed = item.IsClosed,
                        });
                    }
                }
            }
        }

        await unitOfWork.SaveChangesAsync(ct);
        return Unit.Value;
    }
}
