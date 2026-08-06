namespace Application;

public sealed class DeleteJobLevelCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<DeleteJobLevelCommand, Unit>
{
    public async Task<Unit> Handle(DeleteJobLevelCommand cmd, CancellationToken ct)
    {
        var jobLevel = await unitOfWork.Repository<JobLevel>()
            .FindTrackedAsync(j => j.Id == cmd.JobLevelId, ct)
            ?? throw new NotFoundException(ExceptionMessages.NotFound("JobLevel", cmd.JobLevelId));

        var hasActiveUsers = await unitOfWork.Repository<User>()
            .AnyAsync(u => u.JobLevelId == cmd.JobLevelId && u.IsActive, ct);
        if (hasActiveUsers)
            throw new ConflictException("Cấp bậc còn nhân viên đang hoạt động, không thể xóa.");

        jobLevel.IsDeleted = true;
        jobLevel.DeletedAt = DateTimeOffset.UtcNow;

        await unitOfWork.EnsureSaveAsync(ct);
        return Unit.Value;
    }
}
