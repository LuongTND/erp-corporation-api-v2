namespace Application;

public sealed record RegionHoursItem(DayOfWeek DayOfWeek, TimeOnly OpenTime, TimeOnly CloseTime, bool IsClosed);

public sealed record UpsertRegionHoursCommand(
    Guid RegionId,
    IReadOnlyList<RegionHoursItem> Hours
) : IRequest<Unit>;
