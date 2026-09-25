namespace Application;

public sealed class LoginMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<User, UserCredentials>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.Email, src => src.Email)
            .Map(dest => dest.FullName, src => src.FullName)
            .Map(dest => dest.AvatarURL, src => src.AvatarUrl ?? string.Empty)
            .Map(dest => dest.Role, src => src.UserRoles
                .Where(ur => ur.IsActive)
                .Select(ur => ur.Role != null ? ur.Role.RoleName : null)
                .FirstOrDefault());
    }
}
