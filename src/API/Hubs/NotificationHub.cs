namespace API;

[Authorize]
public sealed class NotificationHub : Hub
{
    public const string HubPath = "/hubs/notifications";
    public const string ReceiveNotificationMethod = "ReceiveNotification";
}
