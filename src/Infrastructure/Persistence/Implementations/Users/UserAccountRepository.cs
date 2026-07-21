namespace Infrastructure;

[RegisterService(typeof(IUserAccountRepository))]
public class UserAccountRepository : GenericRepository<UserAccount>, IUserAccountRepository
{
    public UserAccountRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<UserAccount?> GetByEmailWithUserAsync(string email, CancellationToken ct = default)
    {
        return await DbSet
            .Include(a => a.User).ThenInclude(u => u.UserRoles).ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(a => a.LoginEmail == email, ct);
    }

    public async Task<UserAccount?> GetByRefreshTokenWithUserAsync(string refreshToken, CancellationToken ct = default)
    {
        return await DbSet
            .Include(a => a.User).ThenInclude(u => u.UserRoles).ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(a => a.RefreshToken == refreshToken, ct);
    }

    public async Task<UserAccount?> GetByUserIdAsync(Guid userId, CancellationToken ct = default)
    {
        return await DbSet
            .FirstOrDefaultAsync(a => a.UserId == userId, ct);
    }
}