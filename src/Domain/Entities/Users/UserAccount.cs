using System.Security.Cryptography;
using Domain.Base;

namespace Domain.Entities.Users;

public class UserAccount : BaseEntity
{
    public Guid UserId { get; private set; }
    public virtual User User { get; private set; } = null!;

    public string LoginEmail { get; private set; } = null!;
    public string? PasswordHash { get; private set; }
    public bool IsLocked { get; private set; }
    public DateTime? LastLoginAt { get; private set; }
    public int FailedLoginCount { get; private set; }

    public string? RefreshToken { get; private set; }
    public DateTime? RefreshTokenExpiresAt { get; private set; }

    private UserAccount() : base()
    {
    }

    public static UserAccount Create(Guid userId, string loginEmail, string? passwordHash = null)
    {
        return new UserAccount
        {
            UserId = userId,
            LoginEmail = loginEmail.ToLowerInvariant(),
            PasswordHash = passwordHash,
            IsLocked = false,
            FailedLoginCount = 0
        };
    }

    public void UpdateEmail(string loginEmail)
    {
        LoginEmail = loginEmail.ToLowerInvariant();
    }

    public void UpdatePassword(string passwordHash)
    {
        PasswordHash = passwordHash;
        FailedLoginCount = 0;
    }

    public void Lock()
    {
        IsLocked = true;
    }

    public void Unlock()
    {
        IsLocked = false;
        FailedLoginCount = 0;
    }

    public void RecordLoginSuccess()
    {
        LastLoginAt = DateTime.UtcNow;
        FailedLoginCount = 0;
    }

    public void RecordLoginFailure()
    {
        FailedLoginCount++;
        if (FailedLoginCount >= 5)
        {
            IsLocked = true;
        }
    }

    public string SetRefreshToken(int expirationDays)
    {
        var randomBytes = RandomNumberGenerator.GetBytes(64);
        RefreshToken = Convert.ToBase64String(randomBytes);
        RefreshTokenExpiresAt = DateTime.UtcNow.AddDays(expirationDays);
        return RefreshToken;
    }

    public void ClearRefreshToken()
    {
        RefreshToken = null;
        RefreshTokenExpiresAt = null;
    }

    public bool IsRefreshTokenValid(string token)
    {
        return RefreshToken == token
            && RefreshTokenExpiresAt.HasValue
            && RefreshTokenExpiresAt.Value > DateTime.UtcNow;
    }
}
