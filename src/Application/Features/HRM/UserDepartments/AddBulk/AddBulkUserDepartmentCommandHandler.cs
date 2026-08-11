namespace Application;

public sealed class AddBulkUserDepartmentCommandHandler(IUnitOfWork unitOfWork, IUserContext currentUser)
    : IRequestHandler<AddBulkUserDepartmentCommand, int>
{
    public async Task<int> Handle(AddBulkUserDepartmentCommand cmd, CancellationToken ct)
    {
        var dept = await unitOfWork.Repository<Department>()
            .FindAsync(d => d.Id == cmd.DepartmentId && d.IsActive, ct)
            ?? throw new NotFoundException(ExceptionMessages.NotFound("Department", cmd.DepartmentId));

        var userIds = cmd.UserIds.Distinct().ToList();
        if (userIds.Count == 0) return 0;

        var existing = await unitOfWork.Repository<UserDepartment>()
            .GetAllTrackedAsync(ud => ud.DepartmentId == cmd.DepartmentId && userIds.Contains(ud.UserId), ct);

        var existingMap = existing.ToDictionary(ud => ud.UserId);

        int added = 0;
        foreach (var uid in userIds)
        {
            if (existingMap.TryGetValue(uid, out var ud))
            {
                if (ud.IsActive) continue;
                ud.IsActive = true;
                ud.StartDate = cmd.StartDate;
                ud.EndDate = null;
            }
            else
            {
                await unitOfWork.Repository<UserDepartment>().AddAsync(new UserDepartment
                {
                    Id = Guid.NewGuid(),
                    UserId = uid,
                    DepartmentId = cmd.DepartmentId,
                    IsPrimary = false,
                    StartDate = cmd.StartDate,
                    IsActive = true
                });
            }

            await unitOfWork.Repository<WorkHistory>().AddAsync(new WorkHistory
            {
                Id = Guid.NewGuid(),
                UserId = uid,
                ChangeType = WorkHistoryChangeType.Department,
                OldValue = null,
                NewValue = dept.DepartmentName,
                ChangedBy = currentUser.UserId,
                ChangedAt = DateTimeOffset.UtcNow,
            });

            added++;
        }

        if (added == 0) return 0;

        await unitOfWork.EnsureSaveAsync(ct);
        return added;
    }
}
