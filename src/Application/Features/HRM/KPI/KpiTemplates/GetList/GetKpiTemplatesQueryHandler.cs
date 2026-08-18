namespace Application;

public sealed class GetKpiTemplatesQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetKpiTemplatesQuery, QueryResult<KpiTemplateResponse>>
{
    public async Task<QueryResult<KpiTemplateResponse>> Handle(GetKpiTemplatesQuery query, CancellationToken ct)
    {
        var search = query.QueryInfo.SearchText?.Trim().ToLower();

        var result = await unitOfWork.Repository<KpiTemplate>().GetPagedAsync(
            query.QueryInfo,
            filter: t =>
                (search == null || t.Name.ToLower().Contains(search)) &&
                (query.DepartmentId == null || t.DepartmentId == query.DepartmentId) &&
                (query.JobLevelId == null || t.JobLevelId == query.JobLevelId),
            orderBy: q => q.OrderBy(t => t.Name),
            ct: ct);

        var templates2 = result.Items.ToList();
        if (templates2.Count == 0)
            return new QueryResult<KpiTemplateResponse> { Items = [], TotalCount = result.TotalCount };

        var deptIds = templates2.Select(t => t.DepartmentId).Distinct().ToList();
        var levelIds = templates2.Where(t => t.JobLevelId.HasValue).Select(t => t.JobLevelId!.Value).Distinct().ToList();

        var depts = (await unitOfWork.Repository<Department>().GetAllAsync(d => deptIds.Contains(d.Id), ct))
            .ToDictionary(d => d.Id);
        var levels = levelIds.Count > 0
            ? (await unitOfWork.Repository<JobLevel>().GetAllAsync(j => levelIds.Contains(j.Id), ct)).ToDictionary(j => j.Id)
            : new Dictionary<Guid, JobLevel>();

        var metricsByTemplate = (await unitOfWork.Repository<KpiMetric>()
            .GetAllAsync(m => templates2.Select(t => t.Id).Contains(m.TemplateId), ct))
            .GroupBy(m => m.TemplateId)
            .ToDictionary(g => g.Key, g => g.ToList());

        var items = templates2.Select(t => new KpiTemplateResponse
        {
            Id = t.Id,
            Name = t.Name,
            DepartmentId = t.DepartmentId,
            DepartmentName = depts.TryGetValue(t.DepartmentId, out var d) ? d.DepartmentName : string.Empty,
            JobLevelId = t.JobLevelId,
            JobLevelName = t.JobLevelId.HasValue && levels.TryGetValue(t.JobLevelId.Value, out var l) ? l.LevelName : null,
            IsActive = t.IsActive,
            Metrics = metricsByTemplate.TryGetValue(t.Id, out var metrics)
                ? metrics.Select(m => new KpiMetricResponse
                {
                    Id = m.Id,
                    Name = m.Name,
                    Unit = m.Unit,
                    Weight = m.Weight,
                    Target = m.Target,
                    Type = m.Type
                }).ToList()
                : []
        });

        return new QueryResult<KpiTemplateResponse> { Items = items, TotalCount = result.TotalCount };
    }
}
