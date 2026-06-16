namespace Application.Interfaces.Services.Auth;

public interface ICurrentUserService
{
    Guid? UserId { get; }
    Guid? AccountId { get; }
}
