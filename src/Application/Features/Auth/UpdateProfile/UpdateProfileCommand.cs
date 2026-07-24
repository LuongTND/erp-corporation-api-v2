namespace Application;

public sealed record UpdateProfileCommand(string? FullName, string? Email) : IRequest<UserProfileResponse>;
