namespace Application;

public sealed class GetProfileQueryHandler(
    IUnitOfWork unitOfWork,
    IUserContext userContext,
    IMapper mapper,
    IBlobStorageService blobStorage)
    : IRequestHandler<GetProfileQuery, UserProfileResponse>
{
    private const string Container = "avatars";

    public async Task<UserProfileResponse> Handle(GetProfileQuery query, CancellationToken ct)
    {
        var user = await unitOfWork.Repository<User>().Query()
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Id == userContext.UserId, ct)
            ?? throw new NotFoundException(ExceptionMessages.NotFound("User", userContext.UserId));

        var response = mapper.Map<UserProfileResponse>(user);
        response.AvatarUrl = user.AvatarUrl is null ? null : blobStorage.GetUrl(Container, user.AvatarUrl);
        return response;
    }
}
