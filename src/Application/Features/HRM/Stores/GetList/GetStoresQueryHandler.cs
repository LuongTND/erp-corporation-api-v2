namespace Application;

public sealed class GetStoresQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetStoresQuery, QueryResult<StoreResponse>>
{
    public async Task<QueryResult<StoreResponse>> Handle(GetStoresQuery query, CancellationToken ct)
    {
        var search = query.QueryInfo.SearchText?.Trim().ToLower();
        var today = VietnamTime.Today;

        var dbQuery = unitOfWork.Repository<Store>().Query()
            .Include(s => s.StoreHours.Where(h => h.DayOfWeek == today))
            .Where(s => !s.IsDeleted);

        if (query.RegionId.HasValue)
            dbQuery = dbQuery.Where(s => s.RegionId == query.RegionId.Value);

        if (search != null)
            dbQuery = dbQuery.Where(s =>
                s.Name.ToLower().Contains(search) ||
                s.Code.ToLower().Contains(search) ||
                (s.Address != null && s.Address.ToLower().Contains(search)));

        var totalCount = query.QueryInfo.NeedTotalCount
            ? await dbQuery.CountAsync(ct)
            : 0;

        var stores = await dbQuery
            .OrderBy(s => s.Code)
            .Skip(query.QueryInfo.Skip)
            .Take(query.QueryInfo.Top)
            .ToListAsync(ct);

        var items = stores.Select(s => new StoreResponse
        {
            Id = s.Id,
            Name = s.Name,
            Code = s.Code,
            PosStoreId = s.PosStoreId,
            Address = s.Address,
            Phone = s.Phone,
            RegionId = s.RegionId,
            IsActive = s.IsActive,
            TodayIsClosed = s.StoreHours.FirstOrDefault()?.IsClosed,
        });

        return new QueryResult<StoreResponse> { Items = items, TotalCount = totalCount };
    }
}
