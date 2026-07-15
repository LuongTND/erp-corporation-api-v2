
namespace Application;
public interface IUserAccountRepository : IGenericRepository<UserAccount>
{
    Task<UserAccount?> GetByEmailWithUserAsync(string email, CancellationToken ct = default);
    Task<UserAccount?> GetByRefreshTokenWithUserAsync(string refreshToken, CancellationToken ct = default);
    Task<UserAccount?> GetByUserIdAsync(Guid userId, CancellationToken ct = default);
}
