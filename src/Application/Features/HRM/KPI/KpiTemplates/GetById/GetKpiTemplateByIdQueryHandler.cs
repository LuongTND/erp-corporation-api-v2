namespace Application;

public sealed class GetKpiTemplateByIdQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetKpiTemplateByIdQuery, KpiTemplateResponse>
{
    public async Task<KpiTemplateResponse> Handle(GetKpiTemplateByIdQuery query, CancellationToken ct)
    {
        var template = await unitOfWork.Repository<KpiTemplate>()
            .FindAsync(t => t.Id == query.Id, ct,
                t => t.Department,
                t => t.JobLevel!,
                t => t.Metrics)
            ?? throw new NotFoundException(ExceptionMessages.NotFound("KpiTemplate", query.Id));

        return MapToResponse(template);
    }

    internal static KpiTemplateResponse MapToResponse(KpiTemplate t) => new()
    {
        Id = t.Id,
        Name = t.Name,
        DepartmentId = t.DepartmentId,
        DepartmentName = t.Department?.DepartmentName ?? string.Empty,
        JobLevelId = t.JobLevelId,
        JobLevelName = t.JobLevel?.LevelName,
        IsActive = t.IsActive,
        Metrics = t.Metrics.Select(m => new KpiMetricResponse
        {
            Id = m.Id,
            Name = m.Name,
            Unit = m.Unit,
            Weight = m.Weight,
            Target = m.Target,
            Type = m.Type.ToString()
        }).ToList()
    };
}
