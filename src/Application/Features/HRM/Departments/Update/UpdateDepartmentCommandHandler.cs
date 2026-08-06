namespace Application;

public sealed class UpdateDepartmentCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateDepartmentCommand, Unit>
{
    public async Task<Unit> Handle(UpdateDepartmentCommand cmd, CancellationToken ct)
    {
        var dept = await unitOfWork.Repository<Department>()
            .FindTrackedAsync(d => d.Id == cmd.DepartmentId, ct)
            ?? throw new NotFoundException(ExceptionMessages.NotFound("Department", cmd.DepartmentId));

        var code = cmd.DepartmentCode.ToUpperInvariant();

        var codeExists = await unitOfWork.Repository<Department>()
            .AnyAsync(d => d.DepartmentCode == code && d.Id != cmd.DepartmentId, ct);
        if (codeExists)
            throw new ConflictException(ExceptionMessages.AlreadyExists("DepartmentCode", code));

        if (cmd.ParentDepartmentId.HasValue)
        {
            if (cmd.ParentDepartmentId.Value == cmd.DepartmentId)
                throw new BadRequestException("Phòng ban không thể là cha của chính nó.");

            var parent = await unitOfWork.Repository<Department>()
                .FindAsync(d => d.Id == cmd.ParentDepartmentId.Value, ct)
                ?? throw new NotFoundException(ExceptionMessages.NotFound("Department (parent)", cmd.ParentDepartmentId.Value));

            if (!parent.IsActive)
                throw new BadRequestException("Phòng ban cha đã bị vô hiệu hóa.");

            await EnsureNoCycleAsync(cmd.DepartmentId, cmd.ParentDepartmentId.Value, ct);
        }

        if (cmd.ManagerId.HasValue)
        {
            var manager = await unitOfWork.Repository<User>()
                .FindAsync(u => u.Id == cmd.ManagerId.Value, ct)
                ?? throw new NotFoundException(ExceptionMessages.NotFound("User (manager)", cmd.ManagerId.Value));

            if (!manager.IsActive)
                throw new BadRequestException("Trưởng phòng đã bị vô hiệu hóa.");
        }

        dept.DepartmentName = cmd.DepartmentName;
        dept.DepartmentCode = code;
        dept.ParentDepartmentId = cmd.ParentDepartmentId;
        dept.ManagerId = cmd.ManagerId;
        dept.Description = cmd.Description;
        dept.IsActive = cmd.IsActive;

        await unitOfWork.EnsureSaveAsync(ct);
        return Unit.Value;
    }

    private async Task EnsureNoCycleAsync(Guid deptId, Guid newParentId, CancellationToken ct)
    {
        var current = newParentId;
        while (true)
        {
            var node = await unitOfWork.Repository<Department>()
                .FindAsync(d => d.Id == current, ct);
            if (node is null || node.ParentDepartmentId is null) return;
            if (node.ParentDepartmentId.Value == deptId)
                throw new ConflictException("Không thể tạo chu trình trong cây phòng ban.");
            current = node.ParentDepartmentId.Value;
        }
    }
}
