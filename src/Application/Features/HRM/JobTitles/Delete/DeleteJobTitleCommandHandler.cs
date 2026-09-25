namespace Application;

public sealed class DeleteJobTitleCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<DeleteJobTitleCommand, Unit>
{
    public async Task<Unit> Handle(DeleteJobTitleCommand cmd, CancellationToken ct)
    {
        var jobTitle = await unitOfWork.Repository<JobTitle>()
            .FindTrackedAsync(j => j.Id == cmd.JobTitleId, ct)
            ?? throw new NotFoundException($"Không tìm thấy chức danh với ID '{cmd.JobTitleId}'.");

        if (jobTitle.IsDeleted)
            throw new BadRequestException("Chức danh này đã bị xóa.");

        var hasActiveUsers = await unitOfWork.Repository<User>()
            .AnyAsync(u => u.JobTitleId == cmd.JobTitleId && u.IsActive, ct);
        if (hasActiveUsers)
            throw new ConflictException($"Chức danh '{jobTitle.Name}' đang được gán cho nhân viên đang hoạt động, không thể xóa.");

        jobTitle.IsDeleted = true;
        jobTitle.DeletedAt = DateTimeOffset.UtcNow;

        await unitOfWork.EnsureSaveAsync(ct);
        return Unit.Value;
    }
}
