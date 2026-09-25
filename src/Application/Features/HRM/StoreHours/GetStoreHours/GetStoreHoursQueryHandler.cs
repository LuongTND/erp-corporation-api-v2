namespace Application;

public sealed class GetStoreHoursQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetStoreHoursQuery, IEnumerable<StoreHoursResponse>>
{
    public async Task<IEnumerable<StoreHoursResponse>> Handle(GetStoreHoursQuery query, CancellationToken ct)
    {
        var result = await unitOfWork.Repository<StoreHours>().GetPagedAsync(
            new QueryInfo { Top = 7, NeedTotalCount = false },
            filter: h => h.StoreId == query.StoreId,
            orderBy: q => q.OrderBy(h => h.DayOfWeek),
            ct: ct);

        return result.Items.Select(h => new StoreHoursResponse(
            h.Id, h.StoreId, h.DayOfWeek, h.OpenTime, h.CloseTime, h.IsClosed));
    }
}
