namespace Application;

public sealed class GetDepartmentJobLevelsQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetDepartmentJobLevelsQuery, QueryResult<DepartmentJobLevelResponse>>
{
    public async Task<QueryResult<DepartmentJobLevelResponse>> Handle(GetDepartmentJobLevelsQuery query, CancellationToken ct)
    {
        var result = await unitOfWork.Repository<DepartmentJobLevel>().GetPagedAsync(
            query.QueryInfo,
            filter: d => query.DepartmentId == null || d.DepartmentId == query.DepartmentId,
            orderBy: q => q.OrderBy(d => d.DepartmentId).ThenBy(d => d.JobLevelId),
            ct: ct);

        var items2 = result.Items.ToList();
        if (items2.Count == 0)
            return new QueryResult<DepartmentJobLevelResponse> { Items = [], TotalCount = result.TotalCount };

        var deptIds = items2.Select(d => d.DepartmentId).Distinct().ToList();
        var levelIds = items2.Select(d => d.JobLevelId).Distinct().ToList();
        var policyIds = items2.Where(d => d.BonusPolicyId.HasValue).Select(d => d.BonusPolicyId!.Value).Distinct().ToList();
        var templateIds = items2.Where(d => d.KpiTemplateId.HasValue).Select(d => d.KpiTemplateId!.Value).Distinct().ToList();

        var depts = (await unitOfWork.Repository<Department>().GetAllAsync(d => deptIds.Contains(d.Id), ct)).ToDictionary(d => d.Id);
        var levels = (await unitOfWork.Repository<JobLevel>().GetAllAsync(j => levelIds.Contains(j.Id), ct)).ToDictionary(j => j.Id);
        var policies = policyIds.Count > 0
            ? (await unitOfWork.Repository<Domain.BonusPolicy>().GetAllAsync(b => policyIds.Contains(b.Id), ct)).ToDictionary(b => b.Id)
            : new Dictionary<Guid, Domain.BonusPolicy>();
        var templates = templateIds.Count > 0
            ? (await unitOfWork.Repository<KpiTemplate>().GetAllAsync(t => templateIds.Contains(t.Id), ct)).ToDictionary(t => t.Id)
            : new Dictionary<Guid, KpiTemplate>();

        var items = items2.Select(d => new DepartmentJobLevelResponse
        {
            Id = d.Id,
            DepartmentId = d.DepartmentId,
            DepartmentName = depts.TryGetValue(d.DepartmentId, out var dept) ? dept.DepartmentName : string.Empty,
            JobLevelId = d.JobLevelId,
            JobLevelName = levels.TryGetValue(d.JobLevelId, out var level) ? level.LevelName : string.Empty,
            BonusPolicyId = d.BonusPolicyId,
            BonusPolicyName = d.BonusPolicyId.HasValue && policies.TryGetValue(d.BonusPolicyId.Value, out var policy) ? policy.Name : null,
            KpiTemplateId = d.KpiTemplateId,
            KpiTemplateName = d.KpiTemplateId.HasValue && templates.TryGetValue(d.KpiTemplateId.Value, out var tmpl) ? tmpl.Name : null
        });

        return new QueryResult<DepartmentJobLevelResponse> { Items = items, TotalCount = result.TotalCount };
    }
}
