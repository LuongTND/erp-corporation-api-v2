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

        var existing = await unitOfWork.Repository<RolePermission>().GetPagedAsync(
            new QueryInfo { Top = 10000, NeedTotalCount = false },
            filter: rp => rp.RoleId == cmd.RoleId,
            ct: ct);

        foreach (var rp in existing.Items)
            await unitOfWork.Repository<RolePermission>().RemoveAsync(rp);

        foreach (var permId in cmd.PermissionIds.Distinct())
            await unitOfWork.Repository<RolePermission>().AddAsync(new RolePermission
            {
                Id = Guid.NewGuid(),
                RoleId = cmd.RoleId,
                PermissionId = permId,
                AssignedAt = DateTime.UtcNow
            });

        await unitOfWork.EnsureSaveAsync(ct);
        await permissionService.InvalidateCacheAsync(cmd.RoleId);

        var perms = await unitOfWork.Repository<Permission>().GetPagedAsync(
            new QueryInfo { Top = 10000, NeedTotalCount = false },
            filter: p => cmd.PermissionIds.Contains(p.Id), ct: ct);
        var codes = string.Join(", ", perms.Items.Select(p => p.PermissionCode));
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

        return Unit.Value;
    }
}
