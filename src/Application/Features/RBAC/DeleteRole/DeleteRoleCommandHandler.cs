namespace Application;

public sealed class DeleteRoleCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<DeleteRoleCommand, Unit>
{
    public async Task<Unit> Handle(DeleteRoleCommand cmd, CancellationToken ct)
    {
        var role = await unitOfWork.Repository<Role>()
            .FindAsync(r => r.Id == cmd.RoleId, ct)
            ?? throw new NotFoundException(ExceptionMessages.NotFound("Role", cmd.RoleId));

        if (role.IsSystemRole)
            throw new BadRequestException("Không thể xóa role hệ thống.");

        var activeUserRoles = await unitOfWork.Repository<UserRole>()
            .GetAllAsync(ur => ur.RoleId == cmd.RoleId && ur.IsActive, ct);

        if (activeUserRoles.Any())
        {
            if (!cmd.Force)
                throw new ConflictException("Role đang được gán cho người dùng, không thể xóa.");

            await unitOfWork.Repository<UserRole>().RemoveRangeAsync(activeUserRoles);
        }

        await unitOfWork.Repository<Role>().RemoveAsync(role);
        await unitOfWork.EnsureSaveAsync(ct);
        return Unit.Value;
    }
}
