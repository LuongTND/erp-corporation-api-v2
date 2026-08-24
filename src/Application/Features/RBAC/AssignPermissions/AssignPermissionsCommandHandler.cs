namespace Application;

public sealed class AssignPermissionsCommandHandler(IUnitOfWork unitOfWork, IUserContext userContext, IPermissionService permissionService, IPermissionAuditLogRepository permissionAuditLog)
    : IRequestHandler<AssignPermissionsCommand, Unit>
{
    public async Task<Unit> Handle(AssignPermissionsCommand cmd, CancellationToken ct)
    {
        var role = await unitOfWork.Repository<Role>()
            .FindAsync(r => r.Id == cmd.RoleId, ct)
            ?? throw new NotFoundException(ExceptionMessages.NotFound("Role", cmd.RoleId));

        if (role.IsSystemRole)
            throw new BadRequestException("Không thể chỉnh sửa quyền của role hệ thống.");

        foreach (var permId in cmd.ToRemove.Distinct())
        {
            var rp = await unitOfWork.Repository<RolePermission>()
                .FindAsync(x => x.RoleId == cmd.RoleId && x.PermissionId == permId, ct);
            if (rp is not null)
                await unitOfWork.Repository<RolePermission>().RemoveAsync(rp);
        }

        foreach (var permId in cmd.ToAdd.Distinct())
        {
            var exists = await unitOfWork.Repository<RolePermission>()
                .AnyAsync(x => x.RoleId == cmd.RoleId && x.PermissionId == permId, ct);
            if (!exists)
                await unitOfWork.Repository<RolePermission>().AddAsync(new RolePermission
                {
                    Id = Guid.NewGuid(),
                    RoleId = cmd.RoleId,
                    PermissionId = permId,
                    AssignedAt = DateTime.UtcNow
                });
        }

        await unitOfWork.SaveChangesAsync(ct);
        await permissionService.InvalidateCacheAsync(cmd.RoleId);

        var allChanged = cmd.ToAdd.Concat(cmd.ToRemove).Distinct().ToList();
        if (allChanged.Count > 0)
        {
            var perms = await unitOfWork.Repository<Permission>().GetAllAsync(
                p => allChanged.Contains(p.Id), ct);
            var codes = string.Join(", ", perms.Select(p => p.PermissionCode));
            var actor = await unitOfWork.Repository<User>().FindAsync(u => u.Id == userContext.UserId, ct);
            await permissionAuditLog.WriteAsync(new PermissionAuditLog
            {
                Action = "AssignPermissions",
                ActorId = userContext.UserId,
                ActorName = actor?.FullName ?? "System",
                RoleId = cmd.RoleId,
                RoleName = role.RoleName,
                PermissionCodes = codes,
                OccurredAt = DateTimeOffset.UtcNow,
            }, ct);
            await unitOfWork.SaveChangesAsync(ct);
        }

        return Unit.Value;
    }
}
