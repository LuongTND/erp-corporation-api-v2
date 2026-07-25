namespace API;

public sealed class UserIdProvider : IUserIdProvider
{
    // MapInboundClaims = false → "sub" is not mapped to ClaimTypes.NameIdentifier
    public string? GetUserId(HubConnectionContext connection)
        => connection.User?.FindFirstValue("sub");
}
