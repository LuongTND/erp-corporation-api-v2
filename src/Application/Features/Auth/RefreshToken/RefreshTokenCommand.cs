namespace Application;

public sealed record RefreshTokenCommand(string AccessToken, string RefreshToken) : IRequest<SignInResponse>;
