namespace Application;

public sealed record RefreshTokenCommand(string RefreshToken) : IRequest<SignInResponse>;
