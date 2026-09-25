namespace Application;

public sealed class GetProfileQueryHandler(
    IUnitOfWork unitOfWork,
    IUserContext userContext,
    IBlobStorageService blobStorage)
    : IRequestHandler<GetProfileQuery, UserProfileResponse>
{
    private const string Container = "avatars";

    public async Task<UserProfileResponse> Handle(GetProfileQuery query, CancellationToken ct)
    {
        var response = await unitOfWork.Repository<User>().Query()
            .Where(u => u.Id == userContext.UserId)
            .Select(u => new UserProfileResponse
            {
                Id = u.Id,
                FullName = u.FullName,
                Email = u.Email,
                Status = u.Status.ToString(),
                Role = u.UserRoles
                    .Where(ur => ur.IsActive)
                    .Select(ur => ur.Role != null ? ur.Role.RoleName : null)
                    .FirstOrDefault(),
                RoleIds = u.UserRoles
                    .Where(ur => ur.IsActive && ur.RevokedAt == null
                                 && (ur.ExpiresAt == null || ur.ExpiresAt > DateTimeOffset.UtcNow))
                    .Select(ur => ur.RoleId)
                    .ToList(),
                AvatarUrl = u.AvatarUrl,
                LastLoginAt = u.UserAccount != null ? u.UserAccount.LastLoginAt : null,
                EmailVerified = u.UserAccount != null && u.UserAccount.EmailVerified
            })
            .FirstOrDefaultAsync(ct)
            ?? throw new NotFoundException(ExceptionMessages.NotFound("User", userContext.UserId));

        if (!string.IsNullOrEmpty(response.AvatarUrl))
            response.AvatarUrl = blobStorage.GetUrl(Container, response.AvatarUrl);

        return response;
    }
}
