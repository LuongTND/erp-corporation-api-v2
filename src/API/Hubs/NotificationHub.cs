using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;

namespace API;

[Authorize]
public class NotificationHub : Hub
{
    public const string HubPath = "/hubs/notifications";
    public const string ReceiveNotificationMethod = "ReceiveNotification";
    public const string UnreadCountUpdatedMethod = "UnreadCountUpdated";
}

public class UserIdHubProvider : IUserIdProvider
{
    public string? GetUserId(HubConnectionContext connection)
    {
        return connection.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    }
}