namespace Application;

public sealed record StoreHoursItem(DayOfWeek DayOfWeek, TimeOnly OpenTime, TimeOnly CloseTime, bool IsClosed);

public sealed record UpsertStoreHoursCommand(
    Guid StoreId,
    IReadOnlyList<StoreHoursItem> Hours
) : IRequest<Unit>;
