namespace Application;

public sealed class GetProfileQueryHandler(
    IUnitOfWork unitOfWork,
    IUserContext userContext,
    IMapper mapper)
    : IRequestHandler<GetProfileQuery, UserProfileResponse>
{
    public async Task<UserProfileResponse> Handle(GetProfileQuery query, CancellationToken ct)
    {
        var user = await unitOfWork.Repository<User>()
            .FindAsync(u => u.Id == userContext.UserId, ct, u => u.UserRoles)
            ?? throw new NotFoundException(ExceptionMessages.NotFound("User", userContext.UserId));

        return mapper.Map<UserProfileResponse>(user);
    }
}
