namespace Application;

public sealed class UpsertKpiEntryCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<UpsertKpiEntryCommand, Guid>
{
    public async Task<Guid> Handle(UpsertKpiEntryCommand cmd, CancellationToken ct)
    {
        var userExists = await unitOfWork.Repository<User>()
            .AnyAsync(u => u.Id == cmd.UserId && u.IsActive, ct);
        if (!userExists)
            throw new NotFoundException(ExceptionMessages.NotFound("User", cmd.UserId));

        var metricExists = await unitOfWork.Repository<KpiMetric>()
            .AnyAsync(m => m.Id == cmd.KpiMetricId, ct);
        if (!metricExists)
            throw new NotFoundException(ExceptionMessages.NotFound("KpiMetric", cmd.KpiMetricId));

        var existing = await unitOfWork.Repository<KpiEntry>()
            .FindTrackedAsync(k =>
                k.UserId == cmd.UserId &&
                k.KpiMetricId == cmd.KpiMetricId &&
                k.Month == cmd.Month &&
                k.Year == cmd.Year, ct);

        if (existing is not null)
        {
            existing.ActualValue = cmd.ActualValue;
            existing.Score = cmd.Score;
            existing.Note = cmd.Note;
            await unitOfWork.EnsureSaveAsync(ct);
            return existing.Id;
        }

        var entry = new KpiEntry
        {
            Id = Guid.NewGuid(),
            UserId = cmd.UserId,
            KpiMetricId = cmd.KpiMetricId,
            Month = cmd.Month,
            Year = cmd.Year,
            ActualValue = cmd.ActualValue,
            Score = cmd.Score,
            Note = cmd.Note
        };
        await unitOfWork.Repository<KpiEntry>().AddAsync(entry);
        await unitOfWork.EnsureSaveAsync(ct);
        return entry.Id;
    }
}
