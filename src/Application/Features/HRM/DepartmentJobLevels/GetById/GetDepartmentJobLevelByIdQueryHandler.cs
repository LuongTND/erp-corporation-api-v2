namespace Application;

public sealed class GetDepartmentJobLevelByIdQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetDepartmentJobLevelByIdQuery, DepartmentJobLevelResponse>
{
    public async Task<DepartmentJobLevelResponse> Handle(GetDepartmentJobLevelByIdQuery query, CancellationToken ct)
    {
        var djl = await unitOfWork.Repository<DepartmentJobLevel>()
            .FindAsync(d => d.Id == query.Id, ct,
                d => d.Department,
                d => d.JobLevel,
                d => d.BonusPolicy!,
                d => d.KpiTemplate!)
            ?? throw new NotFoundException(ExceptionMessages.NotFound("DepartmentJobLevel", query.Id));

        return MapToResponse(djl);
    }

    internal static DepartmentJobLevelResponse MapToResponse(DepartmentJobLevel d) => new()
    {
        Id = d.Id,
        DepartmentId = d.DepartmentId,
        DepartmentName = d.Department?.DepartmentName ?? string.Empty,
        JobLevelId = d.JobLevelId,
        JobLevelName = d.JobLevel?.LevelName ?? string.Empty,
        BonusPolicyId = d.BonusPolicyId,
        BonusPolicyName = d.BonusPolicy?.Name,
        KpiTemplateId = d.KpiTemplateId,
        KpiTemplateName = d.KpiTemplate?.Name
    };
}
