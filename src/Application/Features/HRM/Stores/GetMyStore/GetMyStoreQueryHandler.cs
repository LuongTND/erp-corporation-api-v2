namespace Application;

public sealed class GetMyStoreQueryHandler(IStoreRepository storeRepository, IUserContext userContext)
    : IRequestHandler<GetMyStoreQuery, StorePortalResponse?>
{
    public async Task<StorePortalResponse?> Handle(GetMyStoreQuery _, CancellationToken ct)
    {
        var store = await storeRepository.GetMyStoreAsync(userContext.UserId, ct);

        if (store is null) return null;

        return new StorePortalResponse
        {
            Id = store.Id,
            Name = store.Name,
            Code = store.Code,
            Address = store.Address,
            Phone = store.Phone,
            RegionName = store.Region?.Name,
            IsActive = store.IsActive,
            TodayHours = store.StoreHours.FirstOrDefault() is { } h
                ? new StoreHoursResponse(h.Id, h.StoreId, h.DayOfWeek, h.OpenTime, h.CloseTime, h.IsClosed)
                : null,
            Counters = store.Counters.Select(c => new CounterResponse
            {
                Id = c.Id,
                StoreId = c.StoreId,
                StoreName = store.Name,
                Name = c.Name,
                Code = c.Code,
                IsActive = c.IsActive,
            }),
        };
    }
}
