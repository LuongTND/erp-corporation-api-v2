namespace Application;

public sealed class CreateRoleCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<CreateRoleCommand, Guid>
{
    public async Task<Guid> Handle(CreateRoleCommand cmd, CancellationToken ct)
    {
        var exists = await unitOfWork.Repository<Role>()
            .FindAsync(r => r.RoleName == cmd.RoleName, ct);

        if (exists is not null)
            throw new ConflictException($"Role '{cmd.RoleName}' đã tồn tại.");

        var role = new Role
        {
            Id = Guid.NewGuid(),
            RoleName = cmd.RoleName,
            DisplayName = cmd.DisplayName,
            Description = cmd.Description,
            IsSystemRole = false,
            IsActive = true,
            DefaultDataScope = cmd.DefaultDataScope
        };

        await unitOfWork.Repository<Role>().AddAsync(role);
        await unitOfWork.EnsureSaveAsync(ct);
        return role.Id;
    }
}
