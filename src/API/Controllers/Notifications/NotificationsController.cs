using API.Base;
using API.Filters;
using Application.Common.Models;
using Application.DTOs.Notifications;
using Application.Interfaces.Services.Notifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Notifications;

[Route("api/notifications")]
[Authorize]
public class NotificationsController : BaseApiController
{
    private readonly IUserNotificationService _userNotificationService;

    public NotificationsController(IUserNotificationService userNotificationService)
    {
        _userNotificationService = userNotificationService;
    }

    [HttpGet]
    public async Task<ActionResult<PaginatedResult<UserNotificationDto>>> GetMyNotifications(
        [FromQuery] PaginationQuery query,
        [FromQuery] bool? isRead,
        CancellationToken ct)
    {
        var result = await _userNotificationService.GetMyPagedAsync(query, isRead, ct);
        return Ok(result);
    }

    [HttpGet("unread-count")]
    public async Task<ActionResult<UnreadNotificationCountDto>> GetUnreadCount(CancellationToken ct)
    {
        var result = await _userNotificationService.GetMyUnreadCountAsync(ct);
        return Ok(result);
    }

    [HttpPatch("{id:guid}/read")]
    public async Task<ActionResult<UserNotificationDto>> MarkRead(Guid id, CancellationToken ct)
    {
        var result = await _userNotificationService.MarkReadAsync(id, ct);
        return Ok(result);
    }

    [HttpPatch("read-all")]
    public async Task<IActionResult> MarkAllRead(CancellationToken ct)
    {
        await _userNotificationService.MarkAllReadAsync(ct);
        return NoContent();
    }
}
