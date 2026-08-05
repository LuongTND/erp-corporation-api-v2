namespace Application;

public sealed class DeleteDepartmentCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<DeleteDepartmentCommand, Unit>
{
    public async Task<Unit> Handle(DeleteDepartmentCommand cmd, CancellationToken ct)
    {
        var dept = await unitOfWork.Repository<Department>()
            .FindTrackedAsync(d => d.Id == cmd.DepartmentId, ct)
            ?? throw new NotFoundException(ExceptionMessages.NotFound("Department", cmd.DepartmentId));

        var hasActiveUsers = await unitOfWork.Repository<UserDepartment>()
            .AnyAsync(ud => ud.DepartmentId == cmd.DepartmentId && ud.IsActive, ct);
        if (hasActiveUsers)
            throw new ConflictException("Phòng ban còn nhân viên đang hoạt động, không thể xóa.");

        var hasActiveChildren = await unitOfWork.Repository<Department>()
            .AnyAsync(d => d.ParentDepartmentId == cmd.DepartmentId && d.IsActive, ct);
        if (hasActiveChildren)
            throw new ConflictException("Phòng ban còn phòng con đang hoạt động, không thể xóa.");

        dept.IsDeleted = true;
        dept.DeletedAt = DateTimeOffset.UtcNow;
        dept.IsActive = false;

        await unitOfWork.EnsureSaveAsync(ct);
        return Unit.Value;
    }
}
