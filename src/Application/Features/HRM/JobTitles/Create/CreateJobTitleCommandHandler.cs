namespace Application;

public sealed class CreateJobTitleCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<CreateJobTitleCommand, Guid>
{
    public async Task<Guid> Handle(CreateJobTitleCommand cmd, CancellationToken ct)
    {
        var codeExists = await unitOfWork.Repository<JobTitle>()
            .AnyAsync(j => j.Code == cmd.Code, ct);
        if (codeExists)
            throw new ConflictException($"Mã chức danh '{cmd.Code}' đã tồn tại.");

        var nameExists = await unitOfWork.Repository<JobTitle>()
            .AnyAsync(j => j.Name == cmd.Name, ct);
        if (nameExists)
            throw new ConflictException($"Tên chức danh '{cmd.Name}' đã tồn tại.");

        var jobTitle = new JobTitle
        {
            Id = Guid.NewGuid(),
            Code = cmd.Code,
            Name = cmd.Name,
            Description = cmd.Description,
            Level = cmd.Level,
            UnitType = cmd.UnitType
        };

        await unitOfWork.Repository<JobTitle>().AddAsync(jobTitle);
        await unitOfWork.EnsureSaveAsync(ct);
        return jobTitle.Id;
    }
}
