namespace Contract;

public sealed class UserProfileResponse
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
public string? Role { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTimeOffset? LastLoginAt { get; set; }
    public bool EmailVerified { get; set; }
}
