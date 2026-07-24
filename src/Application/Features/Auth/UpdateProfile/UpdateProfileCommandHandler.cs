namespace Application;

public sealed class UpdateProfileCommandHandler(
    IUnitOfWork unitOfWork,
    IUserContext userContext,
    IMapper mapper)
    : IRequestHandler<UpdateProfileCommand, UserProfileResponse>
{
    public async Task<UserProfileResponse> Handle(UpdateProfileCommand cmd, CancellationToken ct)
    {
        var user = await unitOfWork.Repository<User>()
            .FindTrackedAsync(u => u.Id == userContext.UserId, ct)
            ?? throw new NotFoundException(ExceptionMessages.NotFound("User", userContext.UserId));

        if (cmd.FullName is not null) user.FullName = cmd.FullName;
        if (cmd.Email is not null) user.Email = cmd.Email;
        await unitOfWork.EnsureSaveAsync(ct);

        var updated = await unitOfWork.Repository<User>()
            .FindAsync(u => u.Id == userContext.UserId, ct, u => u.UserRoles)
            ?? throw new NotFoundException(ExceptionMessages.NotFound("User", userContext.UserId));

        return mapper.Map<UserProfileResponse>(updated);
    }
}
