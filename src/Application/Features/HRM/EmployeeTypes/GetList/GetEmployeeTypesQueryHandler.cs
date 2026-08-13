namespace Application;

public sealed class GetEmployeeTypesQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetEmployeeTypesQuery, QueryResult<EmployeeTypeResponse>>
{
    public async Task<QueryResult<EmployeeTypeResponse>> Handle(GetEmployeeTypesQuery query, CancellationToken ct)
    {
        var search = query.QueryInfo.SearchText?.Trim().ToLower();

        var result = await unitOfWork.Repository<EmployeeType>().GetPagedAsync(
            query.QueryInfo,
            filter: e => search == null || e.Name.ToLower().Contains(search) || e.Code.ToLower().Contains(search),
            orderBy: q => q.OrderBy(e => e.Name),
            ct: ct);

        var typeIds = result.Items.Select(e => e.Id).ToList();
        var counts = (await unitOfWork.Repository<User>().GetPagedAsync(
            new QueryInfo { Top = 100000, NeedTotalCount = false },
            filter: u => u.IsActive && u.EmployeeTypeId.HasValue && typeIds.Contains(u.EmployeeTypeId.Value),
            ct: ct)).Items
            .GroupBy(u => u.EmployeeTypeId!.Value)
            .ToDictionary(g => g.Key, g => g.Count());

        return new QueryResult<EmployeeTypeResponse>
        {
            Items = result.Items.Select(e => new EmployeeTypeResponse
            {
                Id = e.Id,
                Name = e.Name,
                Code = e.Code,
                Description = e.Description,
                IsActive = e.IsActive,
                EmployeeCount = counts.GetValueOrDefault(e.Id)
            }),
            TotalCount = result.TotalCount
        };
    }
}
