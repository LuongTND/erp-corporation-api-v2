namespace Application;

public sealed class ProfileMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<User, UserProfileResponse>()
            .Map(dest => dest.Role, src => src.UserRoles
                .Where(ur => ur.IsActive)
                .Select(ur => ur.Role != null ? ur.Role.RoleName : null)
                .FirstOrDefault())
            .Map(dest => dest.Status, src => src.Status.ToString())
            .Ignore(dest => dest.LastLoginAt)
            .Ignore(dest => dest.EmailVerified);
    }
}
