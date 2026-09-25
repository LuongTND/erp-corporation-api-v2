namespace Application;

public sealed class TransferUserDepartmentCommandHandler(IUnitOfWork unitOfWork, IUserContext currentUser)
    : IRequestHandler<TransferUserDepartmentCommand, Unit>
{
    public async Task<Unit> Handle(TransferUserDepartmentCommand cmd, CancellationToken ct)
    {
        var userExists = await unitOfWork.Repository<User>()
            .AnyAsync(u => u.Id == cmd.UserId && u.IsActive, ct);
        if (!userExists)
            throw new NotFoundException(ExceptionMessages.NotFound("User", cmd.UserId));

        var newDept = await unitOfWork.Repository<Department>()
            .FindAsync(d => d.Id == cmd.NewDepartmentId && d.IsActive, ct)
            ?? throw new NotFoundException(ExceptionMessages.NotFound("Department", cmd.NewDepartmentId));

        var currentPrimary = await unitOfWork.Repository<UserDepartment>()
            .FindTrackedAsync(ud => ud.UserId == cmd.UserId && ud.IsPrimary && ud.IsActive, ct);

        if (currentPrimary?.DepartmentId == cmd.NewDepartmentId)
            throw new ConflictException("Nhân viên đã thuộc phòng ban này là phòng chính.");

        await unitOfWork.BeginTransactionAsync(ct);
        try
        {
            if (currentPrimary is not null)
            {
                currentPrimary.EndDate = cmd.TransferDate.AddDays(-1);
                currentPrimary.IsActive = false;
            }

            await unitOfWork.Repository<UserDepartment>().AddAsync(new UserDepartment
            {
                Id = Guid.NewGuid(),
                UserId = cmd.UserId,
                DepartmentId = cmd.NewDepartmentId,
                IsPrimary = true,
                StartDate = cmd.TransferDate,
                IsActive = true
            });

            await unitOfWork.Repository<WorkHistory>().AddAsync(new WorkHistory
            {
                Id = Guid.NewGuid(),
                UserId = cmd.UserId,
                ChangeType = WorkHistoryChangeType.Department,
                OldValue = currentPrimary is not null
                    ? (await unitOfWork.Repository<Department>().FindAsync(d => d.Id == currentPrimary.DepartmentId, ct))?.DepartmentName
                    : null,
                NewValue = newDept.DepartmentName,
                ChangedBy = currentUser.UserId,
                ChangedAt = DateTimeOffset.UtcNow,
            });

            await unitOfWork.EnsureSaveAsync(ct);
            await unitOfWork.CommitTransactionAsync();
        }
        catch
        {
            await unitOfWork.RollbackTransactionAsync(ct);
            throw;
        }

        return Unit.Value;
    }
}
