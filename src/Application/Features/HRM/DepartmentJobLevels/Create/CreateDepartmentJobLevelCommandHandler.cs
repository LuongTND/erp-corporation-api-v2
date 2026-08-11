namespace Application;

public sealed class CreateDepartmentJobLevelCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<CreateDepartmentJobLevelCommand, Guid>
{
    public async Task<Guid> Handle(CreateDepartmentJobLevelCommand cmd, CancellationToken ct)
    {
        var deptExists = await unitOfWork.Repository<Department>()
            .AnyAsync(d => d.Id == cmd.DepartmentId && d.IsActive, ct);
        if (!deptExists)
            throw new NotFoundException(ExceptionMessages.NotFound("Department", cmd.DepartmentId));

        var levelExists = await unitOfWork.Repository<JobLevel>()
            .AnyAsync(j => j.Id == cmd.JobLevelId && !j.IsDeleted, ct);
        if (!levelExists)
            throw new NotFoundException(ExceptionMessages.NotFound("JobLevel", cmd.JobLevelId));

        var duplicate = await unitOfWork.Repository<DepartmentJobLevel>()
            .AnyAsync(djl => djl.DepartmentId == cmd.DepartmentId && djl.JobLevelId == cmd.JobLevelId, ct);
        if (duplicate)
            throw new ConflictException("Đã có cấu hình lương cho phòng ban và cấp bậc này.");

        if (cmd.BonusPolicyId.HasValue)
        {
            var policyExists = await unitOfWork.Repository<Domain.BonusPolicy>()
                .AnyAsync(b => b.Id == cmd.BonusPolicyId.Value && !b.IsDeleted, ct);
            if (!policyExists)
                throw new NotFoundException(ExceptionMessages.NotFound("BonusPolicy", cmd.BonusPolicyId.Value));
        }

        var djl = new DepartmentJobLevel
        {
            Id = Guid.NewGuid(),
            DepartmentId = cmd.DepartmentId,
            JobLevelId = cmd.JobLevelId,
            BonusPolicyId = cmd.BonusPolicyId
        };

        await unitOfWork.Repository<DepartmentJobLevel>().AddAsync(djl);
        await unitOfWork.EnsureSaveAsync(ct);
        return djl.Id;
    }
}
