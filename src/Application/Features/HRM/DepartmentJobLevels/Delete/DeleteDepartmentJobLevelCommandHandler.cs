namespace Application;

public sealed class DeleteDepartmentJobLevelCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<DeleteDepartmentJobLevelCommand, Unit>
{
    public async Task<Unit> Handle(DeleteDepartmentJobLevelCommand cmd, CancellationToken ct)
    {
        var djl = await unitOfWork.Repository<DepartmentJobLevel>()
            .FindTrackedAsync(d => d.Id == cmd.Id, ct)
            ?? throw new NotFoundException(ExceptionMessages.NotFound("DepartmentJobLevel", cmd.Id));

        // Null out FK on UserDepartments before soft-deleting
        var linkedUds = await unitOfWork.Repository<UserDepartment>()
            .GetAllTrackedAsync(ud => ud.DepartmentJobLevelId == cmd.Id, ct);
        foreach (var ud in linkedUds)
            ud.DepartmentJobLevelId = null;

        djl.IsDeleted = true;
        djl.DeletedAt = DateTimeOffset.UtcNow;

        await unitOfWork.EnsureSaveAsync(ct);
        return Unit.Value;
    }
}
