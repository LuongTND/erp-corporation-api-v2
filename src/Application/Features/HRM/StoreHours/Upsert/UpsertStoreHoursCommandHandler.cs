namespace Application;

public sealed class UpsertStoreHoursCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<UpsertStoreHoursCommand, Unit>
{
    public async Task<Unit> Handle(UpsertStoreHoursCommand cmd, CancellationToken ct)
    {
        var storeExists = await unitOfWork.Repository<Store>()
            .AnyAsync(s => s.Id == cmd.StoreId && !s.IsDeleted, ct);
        if (!storeExists) throw new NotFoundException(ExceptionMessages.NotFound("Store", cmd.StoreId));

        var days = cmd.Hours.Select(h => h.DayOfWeek).ToList();

        var existing = (await unitOfWork.Repository<StoreHours>()
            .GetAllTrackedAsync(h => h.StoreId == cmd.StoreId && days.Contains(h.DayOfWeek), ct))
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
                await unitOfWork.Repository<StoreHours>().AddAsync(new StoreHours
                {
                    Id = Guid.NewGuid(),
                    StoreId = cmd.StoreId,
                    DayOfWeek = item.DayOfWeek,
                    OpenTime = item.OpenTime,
                    CloseTime = item.CloseTime,
                    IsClosed = item.IsClosed,
                });
            }
        }

        await unitOfWork.SaveChangesAsync(ct);
        return Unit.Value;
    }
}
