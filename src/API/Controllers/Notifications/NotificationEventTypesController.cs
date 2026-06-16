using API.Base;
using API.Filters;
using Application.DTOs.Notifications;
using Application.Interfaces.Services.Notifications;
using Domain.Enums.Notifications;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Notifications;

[Route("api/notification-event-types")]
public class NotificationEventTypesController : CrudApiController<
    INotificationEventTypeService,
    NotificationEventTypeDto,
    CreateNotificationEventTypeRequest,
    UpdateNotificationEventTypeRequest>
{
    public NotificationEventTypesController(INotificationEventTypeService service)
        : base(service, new CrudPermissions
        {
            Read = "system.notification.event.read",
            Create = "system.notification.event.create",
            Update = "system.notification.event.update",
            Delete = "system.notification.event.delete",
        })
    {
    }

    [HttpGet("{eventTypeId:guid}/templates")]
    [RequireCrudPermission(CrudOperation.Read)]
    public async Task<ActionResult<IReadOnlyList<NotificationTemplateDto>>> GetTemplates(
        Guid eventTypeId,
        CancellationToken ct)
    {
        var result = await Service.GetTemplatesAsync(eventTypeId, ct);
        return Ok(result);
    }

    [HttpPut("{eventTypeId:guid}/templates/{channel}")]
    [RequireCrudPermission(CrudOperation.Update)]
    public async Task<ActionResult<NotificationTemplateDto>> UpsertTemplate(
        Guid eventTypeId,
        NotificationChannel channel,
        [FromBody] UpsertNotificationTemplateRequest request,
        CancellationToken ct)
    {
        var result = await Service.UpsertTemplateAsync(eventTypeId, channel, request, ct);
        return Ok(result);
    }
}
