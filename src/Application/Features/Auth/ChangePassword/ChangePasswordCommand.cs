namespace Application;

public sealed record ChangePasswordCommand(string CurrentPassword, string NewPassword) : IRequest<MediatR.Unit>;
