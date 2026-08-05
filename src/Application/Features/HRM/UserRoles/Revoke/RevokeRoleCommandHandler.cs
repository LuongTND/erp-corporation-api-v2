namespace Application;

public sealed class RevokeRoleCommandHandler(IUnitOfWork unitOfWork, IUserContext userContext, IPermissionService permissionService)
    : IRequestHandler<RevokeRoleCommand, Unit>
{
    public async Task<Unit> Handle(RevokeRoleCommand cmd, CancellationToken ct)
    {
        var userRole = await unitOfWork.Repository<UserRole>()
            .FindTrackedAsync(ur => ur.UserId == cmd.UserId && ur.RoleId == cmd.RoleId
                                 && ur.IsActive && ur.RevokedAt == null, ct)
            ?? throw new NotFoundException("User không có role này hoặc đã bị thu hồi.");

        userRole.RevokedAt = DateTimeOffset.UtcNow;
        userRole.RevokedBy = userContext.UserId;
        userRole.IsActive = false;

        await unitOfWork.EnsureSaveAsync(ct);
        await permissionService.InvalidateCacheForUserAsync(cmd.UserId);
        return Unit.Value;
    }
}
