namespace Application;

public sealed class CreateDepartmentCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<CreateDepartmentCommand, Guid>
{
    public async Task<Guid> Handle(CreateDepartmentCommand cmd, CancellationToken ct)
    {
        var code = cmd.DepartmentCode.ToUpperInvariant();

        var codeExists = await unitOfWork.Repository<Department>()
            .AnyAsync(d => d.DepartmentCode == code, ct);
        if (codeExists)
            throw new ConflictException(ExceptionMessages.AlreadyExists("DepartmentCode", code));

        if (cmd.ParentDepartmentId.HasValue)
        {
            var parent = await unitOfWork.Repository<Department>()
                .FindAsync(d => d.Id == cmd.ParentDepartmentId.Value, ct)
                ?? throw new NotFoundException(ExceptionMessages.NotFound("Department (parent)", cmd.ParentDepartmentId.Value));

            if (!parent.IsActive)
                throw new BadRequestException("Phòng ban cha đã bị vô hiệu hóa.");
        }

        if (cmd.ManagerId.HasValue)
        {
            var manager = await unitOfWork.Repository<User>()
                .FindAsync(u => u.Id == cmd.ManagerId.Value, ct)
                ?? throw new NotFoundException(ExceptionMessages.NotFound("User (manager)", cmd.ManagerId.Value));

            if (!manager.IsActive)
                throw new BadRequestException("Trưởng phòng đã bị vô hiệu hóa.");
        }

        var dept = new Department
        {
            Id = Guid.NewGuid(),
            DepartmentName = cmd.DepartmentName,
            DepartmentCode = code,
            ParentDepartmentId = cmd.ParentDepartmentId,
            ManagerId = cmd.ManagerId,
            Description = cmd.Description,
            IsActive = true
        };

        await unitOfWork.Repository<Department>().AddAsync(dept);
        await unitOfWork.EnsureSaveAsync(ct);
        return dept.Id;
    }
}
