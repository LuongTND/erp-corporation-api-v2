namespace Application;

public sealed class UpdateJobTitleCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateJobTitleCommand, Unit>
{
    public async Task<Unit> Handle(UpdateJobTitleCommand cmd, CancellationToken ct)
    {
        var jobTitle = await unitOfWork.Repository<JobTitle>()
            .FindTrackedAsync(j => j.Id == cmd.JobTitleId, ct)
            ?? throw new NotFoundException($"Không tìm thấy chức danh với ID '{cmd.JobTitleId}'.");

        if (jobTitle.IsDeleted)
            throw new BadRequestException("Chức danh này đã bị xóa, không thể chỉnh sửa.");

        var codeExists = await unitOfWork.Repository<JobTitle>()
            .AnyAsync(j => j.Code == cmd.Code && j.Id != cmd.JobTitleId, ct);
        if (codeExists)
            throw new ConflictException($"Mã chức danh '{cmd.Code}' đã tồn tại.");

        var nameExists = await unitOfWork.Repository<JobTitle>()
            .AnyAsync(j => j.Name == cmd.Name && j.Id != cmd.JobTitleId, ct);
        if (nameExists)
            throw new ConflictException($"Tên chức danh '{cmd.Name}' đã tồn tại.");

        jobTitle.Code = cmd.Code;
        jobTitle.Name = cmd.Name;
        jobTitle.Description = cmd.Description;
        jobTitle.Level = cmd.Level;
        jobTitle.UnitType = cmd.UnitType;

        await unitOfWork.EnsureSaveAsync(ct);
        return Unit.Value;
    }
}
