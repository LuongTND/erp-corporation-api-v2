using API.Base;
using API.Filters;
using Application.DTOs.Chat;
using Application.Interfaces.Services.Chat;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Chat;

public class ConversationsController : BaseApiController
{
    private readonly IConversationService _conversationService;

    public ConversationsController(IConversationService conversationService)
    {
        _conversationService = conversationService;
    }

    [HttpGet]
    [AuthorizePermission("chat.conversation.read")]
    public async Task<ActionResult<List<ConversationDto>>> GetUserConversations(CancellationToken ct)
    {
        var list = await _conversationService.GetUserConversationsAsync(ct);
        return Ok(list);
    }

    [HttpGet("{id:guid}")]
    [AuthorizePermission("chat.conversation.read")]
    public async Task<ActionResult<ConversationDto>> GetById(Guid id, CancellationToken ct)
    {
        var conv = await _conversationService.GetByIdAsync(id, ct);
        return Ok(conv);
    }

    [HttpPost]
    [AuthorizePermission("chat.conversation.create")]
    public async Task<ActionResult<ConversationDto>> Create([FromBody] CreateConversationRequest request, CancellationToken ct)
    {
        var conv = await _conversationService.CreateAsync(request, ct);
        return CreatedAtAction(nameof(GetById), new { id = conv.Id }, conv);
    }

    [HttpPost("direct/{otherUserId:guid}")]
    [AuthorizePermission("chat.conversation.create")]
    public async Task<ActionResult<ConversationDto>> GetOrCreateDirectConversation(Guid otherUserId, CancellationToken ct)
    {
        var conv = await _conversationService.GetOrCreateDirectConversationAsync(otherUserId, ct);
        return Ok(conv);
    }

    [HttpPost("{id:guid}/mute")]
    [AuthorizePermission("chat.conversation.update")]
    public async Task<IActionResult> SetMute(Guid id, [FromBody] bool isMuted, CancellationToken ct)
    {
        await _conversationService.SetMuteAsync(id, isMuted, ct);
        return Ok(new { Message = "Cập nhật tắt/bật thông báo thành công." });
    }

    [HttpPost("{id:guid}/archive")]
    [AuthorizePermission("chat.conversation.update")]
    public async Task<IActionResult> SetArchived(Guid id, [FromBody] bool isArchived, CancellationToken ct)
    {
        await _conversationService.SetArchivedAsync(id, isArchived, ct);
        return Ok(new { Message = isArchived ? "Đã lưu trữ cuộc trò chuyện." : "Đã khôi phục cuộc trò chuyện." });
    }

    [HttpPost("{id:guid}/members")]
    [AuthorizePermission("chat.member.manage")]
    public async Task<IActionResult> AddMembers(Guid id, [FromBody] List<Guid> userIds, CancellationToken ct)
    {
        await _conversationService.AddMembersAsync(id, userIds, ct);
        return Ok(new { Message = "Thêm thành viên thành công." });
    }

    [HttpDelete("{id:guid}/members/{userId:guid}")]
    [AuthorizePermission("chat.member.manage")]
    public async Task<IActionResult> RemoveMember(Guid id, Guid userId, CancellationToken ct)
    {
        await _conversationService.RemoveMemberAsync(id, userId, ct);
        return NoContent();
    }
}
