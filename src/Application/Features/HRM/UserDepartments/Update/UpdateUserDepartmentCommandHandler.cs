namespace Application;

public sealed class UpdateUserDepartmentCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateUserDepartmentCommand, Unit>
{
    public async Task<Unit> Handle(UpdateUserDepartmentCommand cmd, CancellationToken ct)
    {
        var ud = await unitOfWork.Repository<UserDepartment>()
            .FindTrackedAsync(x => x.UserId == cmd.UserId && x.DepartmentId == cmd.DepartmentId && x.IsActive, ct)
            ?? throw new NotFoundException("Nhân viên không thuộc phòng ban này.");

        if (cmd.JobTitleId.HasValue)
        {
            var levelExists = await unitOfWork.Repository<JobTitle>()
                .AnyAsync(jl => jl.Id == cmd.JobTitleId.Value && !jl.IsDeleted, ct);
            if (!levelExists)
                throw new NotFoundException(ExceptionMessages.NotFound("JobTitle", cmd.JobTitleId.Value));
        }

        // sync to User.JobTitleId — single source of truth for job title
        var user = await unitOfWork.Repository<User>()
            .FindTrackedAsync(u => u.Id == cmd.UserId, ct);
        if (user is not null)
            user.JobTitleId = cmd.JobTitleId;

        await unitOfWork.EnsureSaveAsync(ct);
        return Unit.Value;
    }
}
