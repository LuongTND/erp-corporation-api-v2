namespace Application;

public sealed class DeleteEmployeeTypeCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<DeleteEmployeeTypeCommand, Unit>
{
    public async Task<Unit> Handle(DeleteEmployeeTypeCommand cmd, CancellationToken ct)
    {
        var employeeType = await unitOfWork.Repository<EmployeeType>()
            .FindTrackedAsync(e => e.Id == cmd.EmployeeTypeId, ct)
            ?? throw new NotFoundException(ExceptionMessages.NotFound("EmployeeType", cmd.EmployeeTypeId));

        if (await unitOfWork.Repository<User>().AnyAsync(u => u.EmployeeTypeId == cmd.EmployeeTypeId && u.IsActive, ct))
            throw new ConflictException("Loại nhân sự còn nhân viên đang hoạt động, không thể xóa.");

        employeeType.IsDeleted = true;
        employeeType.DeletedAt = DateTimeOffset.UtcNow;

        await unitOfWork.EnsureSaveAsync(ct);
        return Unit.Value;
    }
}
