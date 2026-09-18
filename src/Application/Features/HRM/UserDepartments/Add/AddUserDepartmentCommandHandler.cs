namespace Application;

public sealed class AddUserDepartmentCommandHandler(IUnitOfWork unitOfWork, IUserContext currentUser)
    : IRequestHandler<AddUserDepartmentCommand, Guid>
{
    public async Task<Guid> Handle(AddUserDepartmentCommand cmd, CancellationToken ct)
    {
        var userExists = await unitOfWork.Repository<User>()
            .AnyAsync(u => u.Id == cmd.UserId && u.IsActive, ct);
        if (!userExists)
            throw new NotFoundException(ExceptionMessages.NotFound("User", cmd.UserId));

        var deptExists = await unitOfWork.Repository<Department>()
            .AnyAsync(d => d.Id == cmd.DepartmentId && d.IsActive, ct);
        if (!deptExists)
            throw new NotFoundException(ExceptionMessages.NotFound("Department", cmd.DepartmentId));

        var alreadyAssigned = await unitOfWork.Repository<UserDepartment>()
            .AnyAsync(ud => ud.UserId == cmd.UserId && ud.DepartmentId == cmd.DepartmentId && ud.IsActive, ct);
        if (alreadyAssigned)
            throw new ConflictException("Nhân viên đã thuộc phòng ban này.");

        if (cmd.JobTitleId.HasValue)
        {
            var levelExists = await unitOfWork.Repository<JobTitle>()
                .AnyAsync(jl => jl.Id == cmd.JobTitleId.Value && !jl.IsDeleted, ct);
            if (!levelExists)
                throw new NotFoundException(ExceptionMessages.NotFound("JobTitle", cmd.JobTitleId.Value));
        }

        var ud = new UserDepartment
        {
            Id = Guid.NewGuid(),
            UserId = cmd.UserId,
            DepartmentId = cmd.DepartmentId,
            IsPrimary = false,
            StartDate = cmd.StartDate,
            IsActive = true
        };

        await unitOfWork.Repository<UserDepartment>().AddAsync(ud);

        // sync to User.JobTitleId — single source of truth for job title
        if (cmd.JobTitleId.HasValue)
        {
            var user = await unitOfWork.Repository<User>()
                .FindTrackedAsync(u => u.Id == cmd.UserId, ct);
            if (user is not null)
                user.JobTitleId = cmd.JobTitleId;
        }

        var dept = await unitOfWork.Repository<Department>().FindAsync(d => d.Id == cmd.DepartmentId, ct);
        await unitOfWork.Repository<WorkHistory>().AddAsync(new WorkHistory
        {
            Id = Guid.NewGuid(),
            UserId = cmd.UserId,
            ChangeType = WorkHistoryChangeType.Department,
            OldValue = null,
            NewValue = dept?.DepartmentName,
            ChangedBy = currentUser.UserId,
            ChangedAt = DateTimeOffset.UtcNow,
        });

        await unitOfWork.EnsureSaveAsync(ct);
        return ud.Id;
    }
}
