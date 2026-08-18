namespace Application;

public sealed record StoreHoursResponse(
    Guid Id,
    Guid StoreId,
    DayOfWeek DayOfWeek,
    TimeOnly OpenTime,
    TimeOnly CloseTime,
    bool IsClosed);
