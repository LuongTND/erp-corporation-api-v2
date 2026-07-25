namespace Application;

public sealed class ChangePasswordCommandHandler(
    IUnitOfWork unitOfWork,
    IPasswordHasher passwordHasher,
    IUserContext userContext)
    : IRequestHandler<ChangePasswordCommand, MediatR.Unit>
{
    public async Task<MediatR.Unit> Handle(ChangePasswordCommand cmd, CancellationToken ct)
    {
        var account = await unitOfWork.Repository<UserAccount>()
            .FindTrackedAsync(a => a.UserId == userContext.UserId, ct)
            ?? throw new NotFoundException(ExceptionMessages.NotFound("UserAccount", userContext.UserId));

        if (!passwordHasher.Verify(cmd.CurrentPassword, account.PasswordHash ?? string.Empty))
            throw new BadRequestException("Current password is incorrect.");

        account.PasswordHash = passwordHasher.Hash(cmd.NewPassword);
        await unitOfWork.EnsureSaveAsync(ct);

        return MediatR.Unit.Value;
    }
}
