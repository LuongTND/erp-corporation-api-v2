using API.Base;
using API.Filters;
using API.Hubs;
using Application.DTOs.Chat;
using Application.Interfaces.Services.Chat;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace API.Controllers.Chat;

public class MessagesController : BaseApiController
{
    private readonly IMessageService _messageService;
    private readonly IHubContext<ChatHub> _chatHubContext;

    public MessagesController(
        IMessageService messageService,
        IHubContext<ChatHub> chatHubContext)
    {
        _messageService = messageService;
        _chatHubContext = chatHubContext;
    }

    [HttpGet("/api/conversations/{conversationId:guid}/messages")]
    [AuthorizePermission("chat.conversation.read")]
    public async Task<ActionResult<List<MessageDto>>> GetPagedMessages(Guid conversationId, [FromQuery] int page = 1, [FromQuery] int pageSize = 50, CancellationToken ct = default)
    {
        var messages = await _messageService.GetPagedMessagesAsync(conversationId, page, pageSize, ct);
        return Ok(messages);
    }

    [HttpPost("/api/conversations/{conversationId:guid}/messages")]
    [AuthorizePermission("chat.message.create")]
    public async Task<ActionResult<MessageDto>> SendMessage(Guid conversationId, [FromBody] CreateMessageRequest request, CancellationToken ct)
    {
        var message = await _messageService.SendMessageAsync(conversationId, request, ct);
        
        // Broadcast message to conversation group
        await _chatHubContext.Clients.Group(conversationId.ToString()).SendAsync("ReceiveMessage", message, cancellationToken: ct);

        return Ok(message);
    }

    [HttpPut("/api/messages/{id:guid}")]
    [AuthorizePermission("chat.message.update")]
    public async Task<ActionResult<MessageDto>> EditMessage(Guid id, [FromBody] string newContent, CancellationToken ct)
    {
        var message = await _messageService.EditMessageAsync(id, newContent, ct);

        // Broadcast edit event to conversation group
        await _chatHubContext.Clients.Group(message.ConversationID.ToString()).SendAsync("MessageEdited", message, cancellationToken: ct);

        return Ok(message);
    }

    [HttpDelete("/api/messages/{id:guid}")]
    [AuthorizePermission("chat.message.delete")]
    public async Task<IActionResult> DeleteMessage(Guid id, CancellationToken ct)
    {
        await _messageService.DeleteMessageAsync(id, ct);
        
        // Broadcast message delete to all clients
        await _chatHubContext.Clients.All.SendAsync("MessageDeleted", id, cancellationToken: ct);

        return NoContent();
    }

    [HttpPost("/api/messages/{id:guid}/reactions")]
    [AuthorizePermission("chat.message.create")]
    public async Task<ActionResult<MessageReactionDto>> ToggleReaction(Guid id, [FromBody] string reactionType, CancellationToken ct)
    {
        var reaction = await _messageService.ToggleReactionAsync(id, reactionType, ct);

        // Broadcast reaction to all clients
        await _chatHubContext.Clients.All.SendAsync("ReactionToggled", reaction, cancellationToken: ct);

        return Ok(reaction);
    }

    [HttpPost("/api/conversations/{conversationId:guid}/read")]
    [AuthorizePermission("chat.message.create")]
    public async Task<IActionResult> MarkAsRead(Guid conversationId, CancellationToken ct)
    {
        await _messageService.MarkAsReadAsync(conversationId, ct);
        return Ok(new { Message = "Đã đánh dấu đã đọc cuộc hội thoại." });
    }
}
