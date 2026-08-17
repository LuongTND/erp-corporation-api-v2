namespace Application;

public sealed class RemoveUserDepartmentCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<RemoveUserDepartmentCommand, Unit>
{
    public async Task<Unit> Handle(RemoveUserDepartmentCommand cmd, CancellationToken ct)
    {
        var ud = await unitOfWork.Repository<UserDepartment>()
            .FindTrackedAsync(x => x.UserId == cmd.UserId && x.DepartmentId == cmd.DepartmentId && x.IsActive, ct)
            ?? throw new NotFoundException("Nhân viên không thuộc phòng ban này.");

        if (ud.IsPrimary)
            throw new BadRequestException("Không thể xóa phòng ban chính. Dùng chức năng chuyển phòng.");

        ud.IsActive = false;
        ud.EndDate = DateOnly.FromDateTime(DateTime.UtcNow);

        await unitOfWork.EnsureSaveAsync(ct);
        return Unit.Value;
    }
}
