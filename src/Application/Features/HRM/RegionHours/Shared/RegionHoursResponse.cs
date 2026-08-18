namespace Application;

public sealed record RegionHoursResponse(
    Guid Id,
    Guid RegionId,
    DayOfWeek DayOfWeek,
    TimeOnly OpenTime,
    TimeOnly CloseTime,
    bool IsClosed);
