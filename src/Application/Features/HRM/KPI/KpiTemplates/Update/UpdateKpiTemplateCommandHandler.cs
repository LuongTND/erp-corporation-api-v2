namespace Application;

public sealed class UpdateKpiTemplateCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateKpiTemplateCommand, Unit>
{
    public async Task<Unit> Handle(UpdateKpiTemplateCommand cmd, CancellationToken ct)
    {
        var template = await unitOfWork.Repository<KpiTemplate>()
            .FindTrackedAsync(t => t.Id == cmd.Id, ct)
            ?? throw new NotFoundException(ExceptionMessages.NotFound("KpiTemplate", cmd.Id));

        var existingMetrics = await unitOfWork.Repository<KpiMetric>()
            .GetAllTrackedAsync(m => m.TemplateId == cmd.Id, ct);

        await unitOfWork.Repository<KpiMetric>().RemoveRangeAsync(existingMetrics);

        template.Name = cmd.Name;
        template.IsActive = cmd.IsActive;
        template.Metrics = cmd.Metrics.Select(m => new KpiMetric
        {
            Id = Guid.NewGuid(),
            TemplateId = cmd.Id,
            Name = m.Name,
            Unit = m.Unit,
            Weight = m.Weight,
            Target = m.Target,
            Type = m.Type
        }).ToList();

        await unitOfWork.EnsureSaveAsync(ct);
        return Unit.Value;
    }
}
