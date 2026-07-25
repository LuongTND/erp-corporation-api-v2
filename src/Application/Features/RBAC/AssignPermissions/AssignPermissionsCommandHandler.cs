namespace Application;

public sealed class AssignPermissionsCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<AssignPermissionsCommand, MediatR.Unit>
{
    public async Task<MediatR.Unit> Handle(AssignPermissionsCommand cmd, CancellationToken ct)
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
        return MediatR.Unit.Value;
    }
}
