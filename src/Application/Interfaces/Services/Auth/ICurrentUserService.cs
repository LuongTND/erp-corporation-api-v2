namespace Application;
public interface ICurrentUserService
{
    Guid? UserId { get; }
    Guid? AccountId { get; }
}
