namespace Application;

public sealed record LoginCommand(string Email, string Password) : IRequest<SignInResponse>;
