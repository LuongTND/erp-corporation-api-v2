using System.Security.Cryptography;

namespace Domain;

public class UserAccount : EntityBase<Guid>
{
    public Guid UserId { get; set; }
    public User? User { get; set; }

    public string LoginEmail { get; set; } = string.Empty;
    public string? PasswordHash { get; set; }
    public bool IsLocked { get; set; }
    public DateTimeOffset? LastLoginAt { get; set; }
    public int FailedLoginCount { get; set; }

    public string? RefreshToken { get; set; }
    public DateTimeOffset? RefreshTokenExpiresAt { get; set; }

    public bool EmailVerified { get; set; } = false;

    public void RecordLoginSuccess()
    {
        LastLoginAt = DateTimeOffset.UtcNow;
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
        RefreshTokenExpiresAt = DateTimeOffset.UtcNow.AddDays(expirationDays);
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
            && RefreshTokenExpiresAt.Value > DateTimeOffset.UtcNow;
    }
}
