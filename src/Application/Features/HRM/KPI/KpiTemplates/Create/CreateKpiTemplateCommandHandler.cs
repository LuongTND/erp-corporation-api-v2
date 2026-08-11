namespace Application;

public sealed class CreateKpiTemplateCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<CreateKpiTemplateCommand, Guid>
{
    public async Task<Guid> Handle(CreateKpiTemplateCommand cmd, CancellationToken ct)
    {
        var deptExists = await unitOfWork.Repository<Department>()
            .AnyAsync(d => d.Id == cmd.DepartmentId && d.IsActive, ct);
        if (!deptExists)
            throw new NotFoundException(ExceptionMessages.NotFound("Department", cmd.DepartmentId));

        if (cmd.JobLevelId.HasValue)
        {
            var levelExists = await unitOfWork.Repository<JobLevel>()
                .AnyAsync(j => j.Id == cmd.JobLevelId.Value && !j.IsDeleted, ct);
            if (!levelExists)
                throw new NotFoundException(ExceptionMessages.NotFound("JobLevel", cmd.JobLevelId.Value));
        }

        var duplicate = await unitOfWork.Repository<KpiTemplate>()
            .AnyAsync(t => t.DepartmentId == cmd.DepartmentId && t.JobLevelId == cmd.JobLevelId, ct);
        if (duplicate)
            throw new ConflictException("Đã có KPI Template cho phòng ban và cấp bậc này.");

        var templateId = Guid.NewGuid();
        var template = new KpiTemplate
        {
            Id = templateId,
            Name = cmd.Name,
            DepartmentId = cmd.DepartmentId,
            JobLevelId = cmd.JobLevelId,
            IsActive = true,
            Metrics = cmd.Metrics.Select(m => new KpiMetric
            {
                Id = Guid.NewGuid(),
                TemplateId = templateId,
                Name = m.Name,
                Unit = m.Unit,
                Weight = m.Weight,
                Target = m.Target,
                Type = m.Type
            }).ToList()
        };

        await unitOfWork.Repository<KpiTemplate>().AddAsync(template);
        await unitOfWork.EnsureSaveAsync(ct);
        return template.Id;
    }
}
