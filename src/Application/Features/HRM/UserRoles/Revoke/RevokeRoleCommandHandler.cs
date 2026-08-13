namespace Application;

public sealed class RevokeRoleCommandHandler(IUnitOfWork unitOfWork, IUserContext userContext, IPermissionService permissionService, IPermissionAuditLogRepository permissionAuditLog)
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

        var role = await unitOfWork.Repository<Role>().FindAsync(r => r.Id == cmd.RoleId, ct);
        var targetUser = await unitOfWork.Repository<User>().FindAsync(u => u.Id == cmd.UserId, ct);
        var actor = await unitOfWork.Repository<User>().FindAsync(u => u.Id == userContext.UserId, ct);
        await permissionAuditLog.WriteAsync(new PermissionAuditLog
        {
            Action = "RevokeRole",
            ActorId = userContext.UserId,
            ActorName = actor?.FullName ?? "System",
            TargetUserId = cmd.UserId,
            TargetUserName = targetUser?.FullName,
            RoleId = cmd.RoleId,
            RoleName = role?.RoleName ?? cmd.RoleId.ToString(),
            OccurredAt = DateTimeOffset.UtcNow,
        }, ct);

        return Unit.Value;
    }
}
