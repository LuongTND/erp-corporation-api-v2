namespace Application;

public sealed class UpdateRoleCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateRoleCommand, Unit>
{
    public async Task<Unit> Handle(UpdateRoleCommand cmd, CancellationToken ct)
    {
        var role = await unitOfWork.Repository<Role>()
            .FindTrackedAsync(r => r.Id == cmd.RoleId, ct)
            ?? throw new NotFoundException(ExceptionMessages.NotFound("Role", cmd.RoleId));

        if (role.IsSystemRole)
            throw new BadRequestException("Không thể chỉnh sửa role hệ thống.");

        role.DisplayName = cmd.DisplayName;
        role.Description = cmd.Description;
        role.DefaultDataScope = cmd.DefaultDataScope;

        await unitOfWork.SaveChangesAsync(ct);
        return Unit.Value;
    }
}
