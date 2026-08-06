namespace Application;

public sealed class BulkSyncRoleUsersCommandHandler(
    IUnitOfWork unitOfWork,
    IUserContext userContext,
    IPermissionService permissionService)
    : IRequestHandler<BulkSyncRoleUsersCommand, Unit>
{
    public async Task<Unit> Handle(BulkSyncRoleUsersCommand cmd, CancellationToken ct)
    {
        var roleExists = await unitOfWork.Repository<Role>()
            .AnyAsync(r => r.Id == cmd.RoleId && r.IsActive, ct);
        if (!roleExists)
            throw new NotFoundException(ExceptionMessages.NotFound("Role", cmd.RoleId));

        // --- Assign ---
        if (cmd.ToAdd.Count > 0)
        {
            var existing = await unitOfWork.Repository<UserRole>()
                .GetAllAsync(ur => ur.RoleId == cmd.RoleId && cmd.ToAdd.Contains(ur.UserId)
                                   && ur.IsActive && ur.RevokedAt == null, ct);
            var existingSet = existing.Select(ur => ur.UserId).ToHashSet();

            var now = DateTimeOffset.UtcNow;
            foreach (var uid in cmd.ToAdd.Where(uid => !existingSet.Contains(uid)))
            {
                await unitOfWork.Repository<UserRole>().AddAsync(new UserRole
                {
                    Id = Guid.NewGuid(),
                    UserId = uid,
                    RoleId = cmd.RoleId,
                    AssignedAt = now,
                    AssignedBy = userContext.UserId,
                    ExpiresAt = cmd.ExpiresAt,
                    IsActive = true
                });
            }
        }

        // --- Revoke ---
        if (cmd.ToRemove.Count > 0)
        {
            var toRevoke = await unitOfWork.Repository<UserRole>()
                .GetAllTrackedAsync(ur => ur.RoleId == cmd.RoleId && cmd.ToRemove.Contains(ur.UserId)
                                          && ur.IsActive && ur.RevokedAt == null, ct);

            var revokedAt = DateTimeOffset.UtcNow;
            foreach (var ur in toRevoke)
            {
                ur.RevokedAt = revokedAt;
                ur.RevokedBy = userContext.UserId;
                ur.IsActive = false;
            }
        }

        await unitOfWork.EnsureSaveAsync(ct);
        await permissionService.InvalidateCacheAsync(cmd.RoleId);

        return Unit.Value;
    }
}
