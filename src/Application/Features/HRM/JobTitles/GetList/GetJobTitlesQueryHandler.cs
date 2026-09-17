namespace Application;

public sealed class GetJobTitlesQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetJobTitlesQuery, QueryResult<JobTitleResponse>>
{
    public async Task<QueryResult<JobTitleResponse>> Handle(GetJobTitlesQuery query, CancellationToken ct)
    {
        var search = query.QueryInfo.SearchText?.Trim().ToLower();

        var result = await unitOfWork.Repository<JobTitle>().GetPagedAsync(
            query.QueryInfo,
            filter: j => search == null || j.Name.ToLower().Contains(search) || j.Code.ToLower().Contains(search),
            orderBy: q => q.OrderBy(j => j.Name),
            ct: ct);

        var levelIds = result.Items.Select(j => j.Id).ToList();
        var usersForCount = (await unitOfWork.Repository<User>().GetPagedAsync(
            new QueryInfo { Top = 100000, NeedTotalCount = false },
            filter: u => u.IsActive && u.JobTitleId.HasValue && levelIds.Contains(u.JobTitleId.Value),
            ct: ct)).Items;
        var counts = usersForCount.GroupBy(u => u.JobTitleId!.Value).ToDictionary(g => g.Key, g => g.Count());

        var items = result.Items.Select(j => new JobTitleResponse
        {
            Id = j.Id,
            Code = j.Code,
            Name = j.Name,
            Description = j.Description,
            Level = j.Level.ToString(),
            UnitType = j.UnitType.ToString(),
            IsDeleted = j.IsDeleted,
            EmployeeCount = counts.GetValueOrDefault(j.Id)
        });

        return new QueryResult<JobTitleResponse> { Items = items, TotalCount = result.TotalCount };
    }
}
