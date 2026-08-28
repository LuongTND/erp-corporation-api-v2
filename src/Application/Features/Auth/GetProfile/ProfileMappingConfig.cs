namespace Application;

public sealed class ProfileMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        // AvatarUrl cần IBlobStorageService — map thủ công trong handler, không dùng Mapster ở đây
        config.NewConfig<User, UserProfileResponse>()
            .Map(dest => dest.Role, src => src.UserRoles
                .Where(ur => ur.IsActive)
                .Select(ur => ur.Role != null ? ur.Role.RoleName : null)
                .FirstOrDefault() ?? string.Empty)
            .Map(dest => dest.Status, src => src.Status.ToString()!)
            .Ignore(dest => dest.AvatarUrl)
            .Ignore(dest => dest.LastLoginAt)
            .Ignore(dest => dest.EmailVerified);
    }
}
