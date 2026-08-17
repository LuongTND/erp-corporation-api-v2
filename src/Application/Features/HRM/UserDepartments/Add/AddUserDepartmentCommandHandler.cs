namespace Application;

public sealed class AddUserDepartmentCommandHandler(IUnitOfWork unitOfWork)
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

        if (cmd.JobLevelId.HasValue)
        {
            var levelExists = await unitOfWork.Repository<JobLevel>()
                .AnyAsync(jl => jl.Id == cmd.JobLevelId.Value && !jl.IsDeleted, ct);
            if (!levelExists)
                throw new NotFoundException(ExceptionMessages.NotFound("JobLevel", cmd.JobLevelId.Value));
        }

        Guid? departmentJobLevelId = null;
        if (cmd.JobLevelId.HasValue)
        {
            var djl = await unitOfWork.Repository<DepartmentJobLevel>()
                .FindAsync(d => d.DepartmentId == cmd.DepartmentId && d.JobLevelId == cmd.JobLevelId.Value, ct);
            departmentJobLevelId = djl?.Id;
        }

        var ud = new UserDepartment
        {
            Id = Guid.NewGuid(),
            UserId = cmd.UserId,
            DepartmentId = cmd.DepartmentId,
            DepartmentJobLevelId = departmentJobLevelId,
            IsPrimary = false,
            StartDate = cmd.StartDate,
            IsActive = true
        };

        await unitOfWork.Repository<UserDepartment>().AddAsync(ud);

        // sync to User.JobLevelId — single source of truth for job title
        if (cmd.JobLevelId.HasValue)
        {
            var user = await unitOfWork.Repository<User>()
                .FindTrackedAsync(u => u.Id == cmd.UserId, ct);
            if (user is not null)
                user.JobLevelId = cmd.JobLevelId;
        }

        await unitOfWork.EnsureSaveAsync(ct);
        return ud.Id;
    }
}
