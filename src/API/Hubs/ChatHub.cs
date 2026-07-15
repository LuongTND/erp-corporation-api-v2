using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace API;

[Authorize]
public class ChatHub : Hub
{
    public const string HubPath = "/hubs/chat";

    public async Task JoinConversation(string conversationId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, conversationId);
    }

    public async Task LeaveConversation(string conversationId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, conversationId);
    }

    public async Task SendTypingStatus(string conversationId, bool isTyping)
    {
        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var fullName = Context.User?.FindFirst(ClaimTypes.Name)?.Value;
        if (!string.IsNullOrEmpty(userId))
        {
            await Clients.Group(conversationId)
                .SendAsync("ReceiveTypingStatus", conversationId, userId, fullName, isTyping);
        }
    }
}