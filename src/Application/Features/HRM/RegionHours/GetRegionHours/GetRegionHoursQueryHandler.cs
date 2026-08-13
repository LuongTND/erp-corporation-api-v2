namespace Application;

public sealed class GetRegionHoursQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetRegionHoursQuery, IEnumerable<RegionHoursResponse>>
{
    public async Task<IEnumerable<RegionHoursResponse>> Handle(GetRegionHoursQuery query, CancellationToken ct)
    {
        var result = await unitOfWork.Repository<RegionHours>().GetPagedAsync(
            new QueryInfo { Top = 7, NeedTotalCount = false },
            filter: h => h.RegionId == query.RegionId,
            orderBy: q => q.OrderBy(h => h.DayOfWeek),
            ct: ct);

        return result.Items.Select(h => new RegionHoursResponse(
            h.Id, h.RegionId, h.DayOfWeek, h.OpenTime, h.CloseTime, h.IsClosed));
    }
}
