namespace Contract;

public sealed class EmailStatus
{
    public required string Email { get; set; }
    public required bool IsVerified { get; set; }
}
