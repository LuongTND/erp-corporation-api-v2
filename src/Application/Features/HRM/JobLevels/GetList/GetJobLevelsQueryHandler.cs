namespace Application;

public sealed class GetJobLevelsQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetJobLevelsQuery, QueryResult<JobLevelResponse>>
{
    public async Task<QueryResult<JobLevelResponse>> Handle(GetJobLevelsQuery query, CancellationToken ct)
    {
        var search = query.QueryInfo.SearchText?.Trim().ToLower();

        var result = await unitOfWork.Repository<JobLevel>().GetPagedAsync(
            query.QueryInfo,
            filter: j => search == null || j.LevelName.ToLower().Contains(search),
            orderBy: q => q.OrderBy(j => j.LevelOrder),
            ct: ct);

        var levelIds = result.Items.Select(j => j.Id).ToList();
        var usersForCount = (await unitOfWork.Repository<User>().GetPagedAsync(
            new QueryInfo { Top = 100000, NeedTotalCount = false },
            filter: u => u.IsActive && u.JobLevelId.HasValue && levelIds.Contains(u.JobLevelId.Value),
            ct: ct)).Items;
        var counts = usersForCount.GroupBy(u => u.JobLevelId!.Value).ToDictionary(g => g.Key, g => g.Count());

        var items = result.Items.Select(j => new JobLevelResponse
        {
            Id = j.Id,
            LevelName = j.LevelName,
            LevelOrder = j.LevelOrder,
            DefaultScopeType = j.DefaultScopeType.ToString(),
            Description = j.Description,
            IsDeleted = j.IsDeleted,
            EmployeeCount = counts.GetValueOrDefault(j.Id)
        });

        return new QueryResult<JobLevelResponse> { Items = items, TotalCount = result.TotalCount };
    }
}
