namespace Application;

public sealed class GetRegionsQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetRegionsQuery, QueryResult<RegionResponse>>
{
    public async Task<QueryResult<RegionResponse>> Handle(GetRegionsQuery query, CancellationToken ct)
    {
        var search = query.QueryInfo.SearchText?.Trim().ToLower();

        var dbQuery = unitOfWork.Repository<Region>().Query()
            .Include(r => r.Stores)
            .Where(r => !r.IsDeleted);

        if (search != null)
            dbQuery = dbQuery.Where(r =>
                r.Name.ToLower().Contains(search) ||
                r.Code.ToLower().Contains(search));

        var totalCount = query.QueryInfo.NeedTotalCount
            ? await dbQuery.CountAsync(ct)
            : 0;

        var regions = await dbQuery
            .OrderBy(r => r.Code)
            .Skip(query.QueryInfo.Skip)
            .Take(query.QueryInfo.Top)
            .ToListAsync(ct);

        var items = regions.Select(r => new RegionResponse
        {
            Id = r.Id,
            Name = r.Name,
            Code = r.Code,
            PosRegionId = r.PosRegionId,
            IsActive = r.IsActive,
            StoreCount = r.Stores.Count,
        });

        return new QueryResult<RegionResponse> { Items = items, TotalCount = totalCount };
    }
}
