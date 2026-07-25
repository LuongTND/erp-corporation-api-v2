namespace Contract;

public sealed class UserCredentials
{
    public required Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string AvatarURL { get; set; } = string.Empty;
    public bool IsVerified { get; set; }
    public string? Role { get; set; }
}
