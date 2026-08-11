namespace Application;

public sealed class DeleteKpiTemplateCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<DeleteKpiTemplateCommand, Unit>
{
    public async Task<Unit> Handle(DeleteKpiTemplateCommand cmd, CancellationToken ct)
    {
        var template = await unitOfWork.Repository<KpiTemplate>()
            .FindTrackedAsync(t => t.Id == cmd.Id, ct)
            ?? throw new NotFoundException(ExceptionMessages.NotFound("KpiTemplate", cmd.Id));

        var inUse = await unitOfWork.Repository<DepartmentJobLevel>()
            .AnyAsync(djl => djl.KpiTemplateId == cmd.Id, ct);
        if (inUse)
            throw new ConflictException("KPI Template đang được sử dụng bởi DepartmentJobLevel, không thể xóa.");

        template.IsDeleted = true;
        template.DeletedAt = DateTimeOffset.UtcNow;

        await unitOfWork.EnsureSaveAsync(ct);
        return Unit.Value;
    }
}
