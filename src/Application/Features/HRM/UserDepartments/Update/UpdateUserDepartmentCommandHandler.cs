namespace Application;

public sealed class UpdateUserDepartmentCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateUserDepartmentCommand, Unit>
{
    public async Task<Unit> Handle(UpdateUserDepartmentCommand cmd, CancellationToken ct)
    {
        var ud = await unitOfWork.Repository<UserDepartment>()
            .FindTrackedAsync(x => x.UserId == cmd.UserId && x.DepartmentId == cmd.DepartmentId && x.IsActive, ct)
            ?? throw new NotFoundException("Nhân viên không thuộc phòng ban này.");

        if (cmd.JobLevelId.HasValue)
        {
            var levelExists = await unitOfWork.Repository<JobLevel>()
                .AnyAsync(jl => jl.Id == cmd.JobLevelId.Value && !jl.IsDeleted, ct);
            if (!levelExists)
                throw new NotFoundException(ExceptionMessages.NotFound("JobLevel", cmd.JobLevelId.Value));
        }

        ud.JobLevelId = cmd.JobLevelId;
        await unitOfWork.EnsureSaveAsync(ct);
        return Unit.Value;
    }
}
