namespace Application;

public sealed class GetCountersQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetCountersQuery, QueryResult<CounterResponse>>
{
    public async Task<QueryResult<CounterResponse>> Handle(GetCountersQuery query, CancellationToken ct)
    {
        var search = query.QueryInfo.SearchText?.Trim().ToLower();

        var dbQuery = unitOfWork.Repository<Counter>().Query()
            .Include(c => c.Store)
            .Where(c => !c.IsDeleted);

        if (query.StoreId.HasValue)
            dbQuery = dbQuery.Where(c => c.StoreId == query.StoreId.Value);

        if (search != null)
            dbQuery = dbQuery.Where(c =>
                c.Name.ToLower().Contains(search) ||
                c.Code.ToLower().Contains(search));

        var totalCount = query.QueryInfo.NeedTotalCount
            ? await dbQuery.CountAsync(ct)
            : 0;

        var counters = await dbQuery
            .OrderBy(c => c.Store.Code)
            .ThenBy(c => c.Code)
            .Skip(query.QueryInfo.Skip)
            .Take(query.QueryInfo.Top)
            .ToListAsync(ct);

        var items = counters.Select(c => new CounterResponse
        {
            Id = c.Id,
            StoreId = c.StoreId,
            StoreName = c.Store.Name,
            Name = c.Name,
            Code = c.Code,
            IsActive = c.IsActive,
        });

        return new QueryResult<CounterResponse> { Items = items, TotalCount = totalCount };
    }
}
