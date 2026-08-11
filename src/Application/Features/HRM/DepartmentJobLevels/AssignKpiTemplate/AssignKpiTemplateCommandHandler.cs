namespace Application;

public sealed class AssignKpiTemplateCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<AssignKpiTemplateCommand, Unit>
{
    public async Task<Unit> Handle(AssignKpiTemplateCommand cmd, CancellationToken ct)
    {
        var djl = await unitOfWork.Repository<DepartmentJobLevel>()
            .FindTrackedAsync(d => d.Id == cmd.DepartmentJobLevelId, ct)
            ?? throw new NotFoundException(ExceptionMessages.NotFound("DepartmentJobLevel", cmd.DepartmentJobLevelId));

        if (cmd.KpiTemplateId.HasValue)
        {
            var template = await unitOfWork.Repository<KpiTemplate>()
                .FindAsync(t => t.Id == cmd.KpiTemplateId.Value && t.IsActive, ct)
                ?? throw new NotFoundException(ExceptionMessages.NotFound("KpiTemplate", cmd.KpiTemplateId.Value));

            if (template.DepartmentId != djl.DepartmentId)
                throw new BadRequestException("KPI Template không thuộc phòng ban này.");
        }

        djl.KpiTemplateId = cmd.KpiTemplateId;
        await unitOfWork.EnsureSaveAsync(ct);
        return Unit.Value;
    }
}
