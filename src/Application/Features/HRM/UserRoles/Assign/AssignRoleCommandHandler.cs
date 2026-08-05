namespace Application;

public sealed class AssignRoleCommandHandler(IUnitOfWork unitOfWork, IUserContext userContext, IPermissionService permissionService)
    : IRequestHandler<AssignRoleCommand, Guid>
{
    public async Task<Guid> Handle(AssignRoleCommand cmd, CancellationToken ct)
    {
        var userExists = await unitOfWork.Repository<User>()
            .AnyAsync(u => u.Id == cmd.UserId && u.IsActive, ct);
        if (!userExists)
            throw new NotFoundException(ExceptionMessages.NotFound("User", cmd.UserId));

        var role = await unitOfWork.Repository<Role>()
            .FindAsync(r => r.Id == cmd.RoleId && r.IsActive, ct)
            ?? throw new NotFoundException(ExceptionMessages.NotFound("Role", cmd.RoleId));

        var alreadyActive = await unitOfWork.Repository<UserRole>()
            .AnyAsync(ur => ur.UserId == cmd.UserId && ur.RoleId == cmd.RoleId
                         && ur.IsActive && ur.RevokedAt == null, ct);
        if (alreadyActive)
            throw new ConflictException($"User đã có role '{role.RoleName}'.");

        var userRole = new UserRole
        {
            Id = Guid.NewGuid(),
            UserId = cmd.UserId,
            RoleId = cmd.RoleId,
            AssignedAt = DateTimeOffset.UtcNow,
            AssignedBy = userContext.UserId,
            ExpiresAt = cmd.ExpiresAt,
            IsActive = true
        };

        await unitOfWork.Repository<UserRole>().AddAsync(userRole);
        await unitOfWork.EnsureSaveAsync(ct);
        await permissionService.InvalidateCacheAsync(cmd.RoleId);
        return userRole.Id;
    }
}
