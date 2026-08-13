namespace Application;

public sealed class DeletePermissionCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<DeletePermissionCommand, Unit>
{
    public async Task<Unit> Handle(DeletePermissionCommand cmd, CancellationToken ct)
    {
        var permission = await unitOfWork.Repository<Permission>()
            .FindAsync(p => p.Id == cmd.PermissionId, ct)
            ?? throw new NotFoundException(ExceptionMessages.NotFound("Permission", cmd.PermissionId));

        await unitOfWork.Repository<Permission>().RemoveAsync(permission);
        await unitOfWork.EnsureSaveAsync(ct);
        return Unit.Value;
    }
}
